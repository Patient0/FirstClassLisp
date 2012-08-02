### First Class Lisp

First Class Lisp is a 'toy' Lisp interpreter written in C#.

It is a Lisp-1 with the following "interesting" features:

* Pattern matching lambda
* First class macros
* First class continuations
* Properly tail recursive
* Clojure style .Net binding

I wrote it mainly for fun, and to experiment with the idea of [First Class macros] (http://matt.might.net/articles/metacircular-evaluation-and-first-class-run-time-macros/) and the [amb operator] (http://mihai.bazon.net/blog/amb-in-javascript).

To try out all of these features and get some idea of performance, I implemented the 'amb' operator as a first-class macro, and re-implemented Peter Norvig's Sudoku algorithm, but using the 'amb' operator to perform the depth-first search.

### Pattern-matching lambda

Standard scheme syntax allows only the following three forms for arguments:


```
(lambda x ...) ' list args
(lambda (a b) ...) ' positional args
(lambda (a b . c) ...) ' some combination
```

In First Class Lisp, the format is:

```
(lambda pattern1 body1
        pattern2 body2
        ...)
```

As well as the usual variations of argument patterns, you can also arbitarily nest them and include constant expressions and quoted symbols. For example, here are the definitions of some of the standard scheme functions in First Class Lisp:

```
(define car (lambda ((a . b)) a))
(define cdr (lambda ((c . d)) d))
(define pair? (lambda ((_ . _)) #t _ #f))
(define nil? (lambda (()) #t _ #f))
```

I did this because it was actually easier - once you've written the code to match the standard argument patterns, it's a simple generalization to make it match arbitrarily deep argument specifications.
The only disadvantage to adding pattern matching this way is that 'lambda' no longer has an implicit 'begin'. 

I also added support for matching against literals including symbol literals (by quoting them). This was also not too hard, and greatly simplified the implementation of the quasiquote macro:

```
(define expand-quasiquote
    (lambda
        (('unquote e))
            e
        ((('unquote-splicing x) . y))
            (list append x (expand-quasiquote y))
        ((x . y))
            (list cons (expand-quasiquote x) (expand-quasiquote y))
        x
            (cons quote x)))
(define quasiquote
    (macro expand-quasiquote))
```

I was even able to implement the core form `if` as a macro:

```
(define if (macro
    (lambda (condition true-case false-case)
            ; Base case. Anything not false is considered 'true'
                `((,lambda (#f) ,false-case
                            _   ,true-case) ,condition))))
```

### Why first class macros?

I wanted to have macros in my Lisp interpreter - but I didn't want to implement fully-fledged Scheme hygienic macros ("syntax-rules"), which I still find difficult to understand, let alone implement!

First class macros provide an easy but elegant way to solve the [hygiene] (http://en.wikipedia.org/wiki/Hygienic_macro) problem.

In a conventional lisp you might define, say, an [arc](http://ycombinator.com/arc/tut.txt) style "let" macro as follows:

```
(define-macro let (var value body)
    `(lambda (,var) ,body) ,value))
```

This would expand:

```
(let x 5 (* x x))
```
into
```
((lambda (x) (* x x) 5)
```

This can cause problems though in that we are relying on `lambda` not having some alternative meaning in the scope in which the let statement was expanded
```
(define (energy lambda)
 (with (c speed-of-light
        h plancks-constant)
  (/ (* c h) lambda)))
```

I got the above example from [Matt Might's page on first class macros](http://matt.might.net/articles/metacircular-evaluation-and-first-class-run-time-macros/) which is a very good read (as are most articles on his page).
In fact, Matt's page was my original inspiration for implementing macros this way.

In First Class Lisp, the 'let' macro is very similar to the one I described earlier, but with one small change:
```
(define-macro let (var value body)
    `(,lambda (,var) ,body) ,value))
```

See the "," before lambda? This is how we solve the hygiene problem: we are unquoting 'lambda' to evaluate it in the environment of the macro expansion rather than the 'calling' context.

This is made possible by the fact that *all* special forms in FirstClass Lisp are actually just [fexprs](http://en.wikipedia.org/wiki/Fexpr) defined in the current lexical environment.
Our macros are just special cases of these f-expressions (except macros do not have access to the environment itself to perform their expansion).

Because all of these objects are mapped to names in the current environment, they can be passed and returned from functions like ordinary objects:

```
(with (identity (lambda (x) x))
           ((identity let) x 3
                (* x x)))
```

My Lisp interpreter doesn't currently let you define your own f-expressions - only your own macros. Also, 'apply', in my Lisp interpreter is a normal function - it doesn't work with macros or f-expressions.
One could enhance it to support this, but this would mean that 'apply' itself could no longer be a function... I wasn't sure if I wanted to go all the way in this direction.

However, for an example of someone who *has* gone all the way down the rabbit-hole in this direction, I recommend checking out this article about [Kernel](http://mainisusuallyafunction.blogspot.co.uk/2012/04/scheme-without-special-forms.html).

One last thing: because of the more flexible argument syntax for 'lambda', 'let' automatically supports 'destructuring'. For example:

```
(let (first . rest) (compute-some-list)
     (do-something-with first rest))
```

### First class continuations

The simplest way to implement an interpreter is to recursively visit the expression-tree, collecting up the terms.

For example, to implement some sort of arithmetic expression evaluator, you'd usually do something like this:

```
interface IExpression
{
    double evaluate(Environment e);
}

class Multiply : IExpression
{
    IExpression left;
    IExpression right;
    public double evaluate(Environment e)
    {
        return left.evaluate(e) * right.evaluate(e);
    }
}
```

In fact, if you look at the GitHub history, you'll see that I started First Class Lisp in this exact way. It's very simple because you are using the implicit runtime stack to keep track of 'where you are'.

Although it's simple, there are two disadvantages to the implicit approach:

1. Properly 'tail recursive' programs in Lisp ideally should be able to loop in constant space. The implicit approach, however, will encounter stack overflow errors.
2. You can't implement proper continuations which are implemented as pure 'first class' objects.

To resolve these issues, my approach was to use two "stacks" to keep track of the current interpreter state:

1. A stack of 'tasks' that still need to be performed
2. A stack of results which have been calculated

We start by pushing the task 'evaluate the expression' on the stack. We then repeatedly pull the top
'task' off the stack and execute it, until there are no more tasks. Once a 'task' has been fully evaluated, the result should be on the result stack. The trick is, the evaluation of a task can trigger *further* tasks to be evaluated,
and intermediate results can be pushed onto the result stack in the mean time.

The approach is essentially the same as the [Shunting Yard Algorithm](http://en.wikipedia.org/wiki/Shunting-yard_algorithm).

To implement first-class continuations efficiently, only one other thing was required, which was to make the entire structure of both stacks **immutable**:

```
    public interface Continuation
    {
        Continuation PushTask(Task task);
        Continuation PopTask();
        Continuation PushResult(Datum d);
        Continuation PopResult();
        // The current task
        Task Task { get; }
        // The current result
        Datum Result { get; }
    }
```

This made it safe to pass the structure around, keep it as a variable and so on.
A `Task` is simply an object that knows how to get from one Continuation to the next. Here's the main loop of the interpreter:

```
        private static Datum Evaluate(Continuation c)
        {
            while (c.Task != null)
            {
                try
                {
                    c = c.Task.Perform(c.PopTask());
                    c.Statistics.Steps++;
                }
                catch (Exception ex)
                {
                    c = c.ErrorHandler(c, ex);
                }
            }
            return c.Result;
        }
```


### .Net method binding and REPL

With the help of my friend [Tim] (http://www.partario.com/blog/) we've also added some basic .Net method bindings. Here are some examples that show how it is used:

```
FCLisp> (System.String/Format "{0} * {1} is {2}" 4 5 (* 4 5))
-> 4 * 5 is 20
Steps: 507 Expansions: 2 Lookups: 46 Elapsed: 00:00:00.0007447
FCLisp> (.Equals "hello" "hello")
-> True
Steps: 318 Expansions: 1 Lookups: 15 Elapsed: 00:00:00.0004844
FCLisp> (map .ToString '(1 2 3))
-> ("1" "2" "3")
Steps: 1141 Expansions: 1 Lookups: 175 Elapsed: 00:00:00.0012821
FCLisp>
```

This was done by augmenting the way that the reader reads in symbols that contain a "." or a "/".
"System.String/Format" is read in by the reader as "(slash (dot System String))".
".Equals" is read in by the reader as "(dot () Equals)".
"dot" and "slash" are in turn defined as macros which use reflection to invoke the corresponding .Net method:

```
(define-macro dot args
    (match args
           (() method)
                `(,make-instance-method ,(symbol->string method))
           name-parts
                `(,get-type ,@(mapcar symbol->string name-parts))))

; System.Console/WriteLine =>
; (slash (dot System Console) WriteLine) =>
; (get-static-method (get-type "System" "Console") "WriteLine")
(define-macro slash (type method)
           `(,get-static-method ,type ,(symbol->string method)))
```

### REPL

The First Class Lisp "REPL" includes an extremely primitive debugger. The REPL and the debugger are both implemented in Lisp itself.
The "main" method of Lisp.exe simply creates a standard environment and evaluates "REPL.lisp".


```
FCLisp> (map (lambda (x) z) '(1 2 3))
ERROR: Undefined symbol 'z'
(debug) to enter debug repl
FCLisp> (debug)
debug> (trace)
ERROR: Undefined symbol 'z'
Tasks:
        Evaluate '(loop)' (Lisp.REPL.lisp:86)
        Discard result
        RestoreErrorHandler
        Invoke '(lambda (result) (,begin ((,macro (lambda (() body) body ((var expr . bindings) . body) `(,let ,var ,expr (,with* ,bindings (,begin ,@body)))))
() (,begin (,begin (,begin (display result) (log-steps ((dot () get_Elapsed) stop-watch))))))))' with 1 args
        Invoke '(lambda (l) (,begin (define reverse-tail (lambda (so-far ()) so-far (so-far (x . y)) (reverse-tail (cons x so-far) y))) (reverse-tail '() l)))'
with 1 args
        Invoke '(lambda (f so-far (() . rest)) so-far (f so-far ll) (map-tail f (cons (apply f (mapcar car ll)) so-far) (mapcar cdr ll)))' with 3 args
        Evaluate '(mapcar cdr ll)' (LispEngine.Bootstrap.Library.lisp:47)
        Invoke ',cons' with 2 args
        Evaluate 'so-far' ()
        Evaluate 'z' ()
Results:
        (lambda (x) z)
-> ()
Steps: 3527 Expansions: 10 Lookups: 457 Elapsed: 00:00:00.0093698
debug> x ; Evaluate 'x' in the frame where the error occurred
-> 1
Steps: 270 Expansions: 0 Lookups: 15 Elapsed: 00:00:00.0003116
debug> (exit) ; exit debug REPL back into main REPL
-> ()
Steps: 284 Expansions: 0 Lookups: 18 Elapsed: 00:00:29.7198458
```


As you will also notice, the debugger is so primitive that it's essentially useless at this stage. You're probably better off sticking in print statements in the code if your
First Class Lisp program isn't working.

### Sudoku solver

What use are first class continuations?

One example which I always thought was really cool is that they allow you to implement the [amb operator] (http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/).

Various web sites (and [SICP] (http://mitpress.mit.edu/sicp/)) give some example programs that use the amb operator (sentence parsing, solving the N-queens problem) but they all seemed a bit "trivial" and/or useless.
Instead, I've re-implemented [Peter Norvig's Sudoku algorithm] (http://norvig.com/sudoku.html) in First Class Lisp, but using the amb operator to implement the depth-first search.

The basic strategy of a Sudoku solver is to apply all of your "deductive" reasoning rules ("if this square has a 6 then all these other squares can no longer have a 6") up until the point at which there are no more rules to apply.
At this point either you've solved the puzzle, or there are still squares which could have multiple digits.

Whereas Peter's program **explicitly** tries all of the possibilities, in First Class Lisp I use the amb operator to pick "the right choice":

```
    (define (solve grid)
        (if (solved? grid)
            grid
            (with* ((s . digits) (square-to-try grid)
                    d (amb digits))
                   (write-line "Assiging {0} to {1}" d s)
                   (solve (assign! (copy-grid grid) s d)))))
```

The above function translates to:

"If the grid is solved, return the grid.
Otherwise, find a square 's' with more than one possible digit and
*let d be the correct choice amongst the available digits*.
Assign 'd' to square 's' and solve the other squares".

So assuming that we could know in advance what the correct digit is for each square, this is a linear algorithm!

Of course, we can't *really* know the correct square in advance. What actually happens is that the "amb" operator saves the current continuation and returns one of the digits. If it turns out that it was the "wrong" choice (because we got an inconsistency),
we call "(amb)" which indicates that we need to go back and try something else. (amb) restores the previous continuation and resumes from the next possible choice.

Here is what you see what you run "Sudoku.lisp" from the Examples directory:

```
FCLisp> (run "..\\..\\Examples\\Sudoku.lisp")
Solving grid1...
((4) (8) (3) (9) (2) (1) (6) (5) (7))
((9) (6) (7) (3) (4) (5) (8) (2) (1))
((2) (5) (1) (8) (7) (6) (4) (9) (3))
((5) (4) (8) (1) (3) (2) (9) (7) (6))
((7) (2) (9) (5) (6) (4) (1) (3) (8))
((1) (3) (6) (7) (9) (8) (2) (4) (5))
((3) (7) (2) (6) (8) (9) (5) (1) (4))
((8) (1) (4) (2) (5) (3) (7) (6) (9))
((6) (9) (5) (4) (1) (7) (3) (8) (2))
Solving grid2...
With rote deduction we only get to:
((4) (1 6 7 9) (1 2 6 7 9) (1 3 9) (2 3 6 9) (2 6 9) (8) (1 2 3 9) (5))
((2 6 7 8 9) (3) (1 2 5 6 7 8 9) (1 4 5 8 9) (2 4 5 6 9) (2 4 5 6 8 9) (1 2 6 7 9) (1 2 4 9) (1 2 4 6 7 9))
((2 6 8 9) (1 5 6 8 9) (1 2 5 6 8 9) (7) (2 3 4 5 6 9) (2 4 5 6 8 9) (1 2 3 6 9) (1 2 3 4 9) (1 2 3 4 6 9))
((3 7 8 9) (2) (1 5 7 8 9) (3 4 5 9) (3 4 5 7 9) (4 5 7 9) (1 3 5 7 9) (6) (1 3 7 8 9))
((3 6 7 9) (1 5 6 7 9) (1 5 6 7 9) (3 5 9) (8) (2 5 6 7 9) (4) (1 2 3 5 9) (1 2 3 7 9))
((3 6 7 8 9) (4) (5 6 7 8 9) (3 5 9) (1) (2 5 6 7 9) (2 3 5 7 9) (2 3 5 8 9) (2 3 7 8 9))
((2 8 9) (8 9) (2 8 9) (6) (4 5 9) (3) (1 2 5 9) (7) (1 2 4 8 9))
((5) (6 7 8 9) (3) (2) (4 7 9) (1) (6 9) (4 8 9) (4 6 8 9))
((1) (6 7 8 9) (4) (5 8 9) (5 7 9) (5 7 8 9) (2 3 5 6 9) (2 3 5 8 9) (2 3 6 8 9))
Solving using non-deterministic search...
Assiging 8 to 55
Assiging 2 to 54
Assiging 4 to 58
Assiging 6 to 64
Assiging 1 to 1
Assiging 3 to 3
Assiging 2 to 7
Assiging 6 to 4
Assiging 9 to 4
Assiging 9 to 7
Assiging 2 to 4
Assiging 6 to 4
Assiging 9 to 3
Assiging 2 to 5
Assiging 6 to 5
Assiging 9 to 1
Assiging 1 to 3
Assiging 2 to 5
Assiging 6 to 5
Assiging 3 to 3
Assiging 2 to 4
Assiging 6 to 4
Assiging 7 to 64
Assiging 1 to 1
Assiging 3 to 3
Solution:
((4) (1) (7) (3) (6) (9) (8) (2) (5))
((6) (3) (2) (1) (5) (8) (9) (4) (7))
((9) (5) (8) (7) (2) (4) (3) (1) (6))
((8) (2) (5) (4) (3) (7) (1) (6) (9))
((7) (9) (1) (5) (8) (6) (4) (3) (2))
((3) (4) (6) (9) (1) (2) (7) (5) (8))
((2) (8) (9) (6) (4) (3) (5) (7) (1))
((5) (7) (3) (2) (9) (1) (6) (8) (4))
((1) (6) (4) (8) (7) (5) (2) (9) (3))
-> ()
Steps: 11141444 Expansions: 219 Lookups: 1400200 Elapsed: 00:00:04.2279425
FCLisp>
```

To understand the way that "amb" works, you can also just try typing in "(amb)" at the command prompt after it has successfully solved a puzzle, to see if there are any *other* solutions. If the Sudoku puzzle that was supplied is not unique,
you'll get all of the solutions by repeatedly typing `(amb)`.

```
FCLisp> (amb)
Assiging 9 to 3
Assiging 2 to 5
Assiging 6 to 5
Assiging 9 to 1
Assiging 1 to 3
Assiging 2 to 5
Assiging 6 to 5
Assiging 3 to 3
... etc.
Assiging 3 to 3
Assiging 9 to 55
Assiging 2 to 54
Assiging 8 to 54
ERROR: No solution
(debug) to enter debug repl
```

Peter's solution used dictionaries and strings, which work very well natively in Python. Because I didn't have a dictionary class as a builtin and was going to have to write some sort of data structure myself anyway,
I decided to add "vectors" (the Scheme/Lisp name for an array) and use those instead.

I used a vector of integers to represent a partially solved Sudoku board.

Each square corresponds to an index into the vector using the formula `9 * row + column`:

```
0  1  2  |3  4  5  |6  7  8  |
9  10 11 |12 13 14 |15 16 17 |
18 19 20 |21 22 23 |24 25 26 |
---------+---------+---------+
27 28 29 |30 31 32 |33 34 35 |
... and so on.
```

### Performance

My first attempt at the Sudoku solver, once it finally worked, was agonizingly slow. It took about 60 seconds, and 140 million steps, to run "Sudoku.lisp", which merely loads and solves "grid1" and "grid2" in Peter Norvig's article.

I was able to get this time to under 5 seconds, by use of the following optimizations:

#### Macro expansion caching

Whenever a macro is expanded, the expansion is cached inside the Datum that was expanded. The next time the interpreter is asked to expand the same macro against the same input code instance, it re-uses the previous expansion.
This eliminated repeated macro expansions in all cases where macros are used in a manner that would also work for 'compile time' macros, which turned out to be almost all of the time.

In the case of a macro being used as a true "first class" macro - e.g. different macros being expanded against the same code instance - it will simply degrade gracefully but correctly.

#### Symbol lookup caching

Once the macro expansions are out of the way, the next most expensive step is looking up symbols in the environment.

At this stage, I have only managed to adopt the following fairly simple and not quite correct optimization: Whenever we lookup a symbol in an environment, we cache the location of its bound value inside the Symbol datum itself.
If we are asked to resolve the same symbol instance against an environment and the symbol was previously resolved against the same environment or one of its parent environments, we skip to that same location.

This optimization isn't quite robust - but I suspect will only break in "pathological" situations in which a macro expands a source tree into a graph in which different nodes in different lexical scopes refer to the same symbol.
The (commented out) unit test 'symbol-lookup-cache' in EvaluatorTests.lisp documents the way in which it can fail.

I had hoped that it would be possible to pre-process the expression-tree after macro expansion, replacing each symbol reference with its 'lexical location' along the lines of [this excellent paper] (http://www.cs.unm.edu/~williams/cs491/three-imp.pdf).

However, determining the true lexical location of each symbol reference is much more complicated because it depends on the nature of the "first" form that expands each element. For example, consider the following

```
((lambda (x)
    (x)) 5)
```

In a conventional Lisp, we can automatically replace the `x` in the body with some sort of lexical marker - and even replace the entire expression with the constant `5` - because the effect of the `lambda` is essentially hard coded.

We know the effect that `lambda` will have on its environment without actually having to evaluate it.

In First Class Lisp, the meaning of 'lambda' itself depends on the surrounding lexical environment, which may or may not impact which lexical scope that the symbol 'x' is in - this makes it difficult (but probably not impossible?) to optimize these lookups.

### Conclusion and next steps

I have to say I've really enjoyed working on this project! I particularly liked the experience of working with macros. Being able to write a macro any time I found myself writing some boiler plate code has been a trully addictive experience!

I've reached the limits of most of the things that I know how to do easily (i.e. without spending days thinking about it!) - so I'm turning it over to you, open source community, to do with it as you will!

The following are areas in which First Class Lisp could be improved:

#### Performance

Performance is "acceptable" - however it would be great if it was possible to implement some sort of incremental compiler while still preserving first class semantics.
The first class semantics makes this difficult - but I'm not sure that it's actually impossible.

The benchmark: My "optimized" version of the Sudoku solver runs in about 4.5 seconds. In contrast, Peter Norvig's solver solves both puzzles in well under 0.03 seconds!

#### Debugging

The 'shunting yard' algorithm made an easy and flexible machine for evaluating the expressions - but gives a very incomprehensible 'trace' when debugging things, and made it quite hard to
implement any standard debugging features like "resume from the next statement". The problem is that the stack of "Tasks" is too low level - "stepping" feels like stepping through assembly language
rather than source code.

By choosing a different internal representation of the interpreter state I think it would be possible to improve the debugger so that it's something that you would actually want to use.

#### Pull requests welcome!

So, with this all of this in mind: Feel free to make a fork of this repository to add your own enhancements or use this as the basis for some other cool project.

I'd particularly welcome any feedback or suggestions on how to improve it - and a pull request would be even more appreciated!


