; This file contains test cases used in EvaluatorTest.
; The "setup" list is executed once at the beginning.
; "tests" is a list of tests. For each test:
; The first element is the name
; The second is expected result.
; The third is the expression to evaluate.
(setup
    (define life 42))
(tests
    (atom 5 5)
    (boolAtomTrue #t #t)
    (boolAtomFalse #f #f)
    ; Many of these tests require an arbitrary symbol
    ; to be defined. So we'll use 'life' as our symbol,
    ; which is mapped to the integer '42' in the environment.
    (symbol 42 life)
    (identityFunction 5 ((lambda (x) x) 5))
    (constantFunction 6 ((lambda () 6)))
    (lambdaList (1 2 3) ((lambda x x) 1 2 3))
    (list (1 2 3) (list 1 2 3))
    (cons (3 . 4) (cons 3 4))
    (recursiveFunctions (3 4 . 5) (cons 3 (cons 4 5)))
    (apply (3 . 4) (apply cons (list 3 4)))
    (dotArgList 4 ((lambda (x . y) x) 4))
    (carList 4 (car (list 4 5)))
    (cdrList (5) (cdr (list 4 5)))
    (eqTrue? #t
            (eq? 4 4))
    (eqFalse? #f
            (eq? 4 3))
    (eqSymbolTest #t
        (eq? 'somesymbol 'somesymbol))
    (eqSymbolTestFalse #f
        (eq? 'someSymbol 'someOtherSymbol))
    (eqQuotedSymbolWithUnquoted #t
        (let x 'unquote
            (eq? x 'unquote)))
    ; But we don't want eq? to be traversing
    ; arbitrarily large structures for us.
    (eqIsFlat #f
            (eq? (list 1 2) (list 1 2)))
    (ifTrue? 5
            (if #t 5 undefined))
    (ifFalse? 5
            (if #f undefined 5))
    ; Test that anything that isn't '#f' is considered
    ; to be true by our if statement.
    (ifAtom 4
            (if 5 4 0))
    (ifMoreComplicated1 3
            (if (< 3 4) 3 4))
    (ifMoreComplicated2 4
            (if (< 4 3) 3 4))
    ; There was a bug which was exposed if the 'false'
    ; case was, in fact, the expression 'true'
    (ifConvertedToTrue #t
        (if #f 53 #t))
    ; Discovered quite late on that the if
    ; F-expression wasn't actually evaluating either the
    ; true or the false cases!
    (ifExpressionTrue 42 
            (if (eq? life 42) life 23))
    (ifExpressionFalse 42 
            (if (eq? life 98) 5 life))
    ; We'll make our 'if' like a 'cond' - allow multiple
    ; clauses until one matches. Should always be an
    ; odd number of clauses.
    (ifIsLikeCond 42
            (if (eq? life 23) not-this-one
                (eq? life 44) not-this-one
                (eq? life 48) no-not-this-either
                42))
    (ifOnlyEvaluatesNecessaryConditions 42
            (if (eq? life 23) not-this-one
                (eq? life 42) 42
                (undefined-operation) no-worries
                default-also-undefined))
    (quotedList (3 4) '(3 4))
    (append (1 2 3 4)
        (append '(1 2) '(3 4)))
    (appendMultiple (1 2 3 4 5 6)
        (append '(1 2) '(3 4) '(5 6)))
    ; It's very convenient if append works even for a single list
    (appendSingle (1 2)
        (append '(1 2)))
    (appendNone ()
        (append))
    (quotedAtom 3 '3)
    (quotedQuote '3 ''3)
    (quotedSymbol x 'x)
    (let 3 (let x 3 x))
    (letEvaluatesBody #t
            (let x 3 (eq? x 3)))

    ; Example modified from
    ; http://matt.might.net/articles/metacircular-evaluation-and-first-class-run-time-macros/
    (lambda-hygiene
        30000
        (begin
        (define speed-of-light 300000)
        (define plancks-constant 1)
        (define (energy lambda)
         (with (c speed-of-light
                h plancks-constant)
           (/ (* c h) lambda)))
        (energy 10)))

    ; Our nil *only* matches nil. Anything else is "not nil"
    (nil1 #t (nil? '()))
    (nil2 #f (nil? 5))
    (nil3 #f (nil? #f))
    (pair1 #t (pair? '(1 . 2)))
    (pair2 #f (pair? 5))

    ; Using the implicit stack for evaluation here would
    ; lead to stack overflow. But because this code is
    ; properly tail recursive and we are using an explicit
    ; stack, this test runs in constant memory.
    (properTailRecursion
        500500
        (begin
            (define total
                (lambda (so-far 0) so-far
                        (so-far x) (total (+ so-far x) (- x 1))))
            (total 0 1000)))
    ; Now that we've allowed mutable state, we need a way to execute multiple
    ; statements in sequence.
    (begin
        23
        (begin
            (define x 23)
            x))

    ; The forms in a begin statement execute
    ; in their own scoped environment - changes
    ; to that environment are not seen outside
    ; the environment.
    (beginScoping
        24
        (begin
            (define x 24)
            (begin
                (define x 48))
            x))
    (lengthTest 3
        (length '(2 4 6)))

    ; Compute 5! using Y-combinator to perform
    ; the recursion rather than 'define' trick
    (y-combinator-test 120
        (let fac (Y (lambda (fac)
                       (lambda (0) 1
                               (x) (* x (fac (- x 1))))))
            (fac 5)))

    ; We can pass almost anything
    ; that has a name as an argument to a function.
    ; Also, we can return the same as arguments from functions.
    ; Not sure how useful this is in practise!
    (fexprs-are-first-class 9
        (with (identity (lambda (x) x)
               square ((identity lambda) (x) (* x x)))
               (square 3)))

    (macros-are-first-class 9
        (with (identity (lambda (x) x))
               ((identity let) x 3
                    (* x x))))

    (not #t
        (not (< 4 3)))

    ; The difference between set! and define had never been
    ; clear to me before now: set! can change things that
    ; may be defined at a higher level. "define" introduces
    ; a new binding at the current level.
    (testNestedSet! 26
        (begin
            (define x 25)
            (begin
                (set! x 26))
            x))

    ; Our "set!" really has to mutate
    ; the underlying values in frames
    ; that may be shared. Easiest way
    ; I could think of to test this:
    ; Write an 'object-like' stack
    ; that has implicit state.
    (testStack 4
        (begin
            (define s (make-stack))
            (s 'push 3)
            (s 'push 4)
            (s 'push 5)
            (s 'pop)
            (s 'pop)))

    (symbol-to-string "hello"
        (symbol->string 'hello))

    (string-to-symbol hello
        (string->symbol "hello"))

 
    ; error-handler provides a way to map from
    ; .NET exceptions back to the pure 'Lisp' world
    ; where a continuation can be invoked.
    (error-translator
           "ERROR"
           (let/cc return
                (execute-with-error-translator
                    (lambda ex (return "ERROR"))
                    (make-thunk undefined-symbol))))

    (nested-error
        "ERROR"
        (try
            (* 6 undefined)
        catch x "ERROR"))

    ; This test checks that we 'remember' to restore
    ; the old error handler if an error happens
    (nested-try-catch
        "ERROR2"
        (try
            (try
                undefined1
            catch msg "ERROR1")
            undefined2
        catch msg "ERROR2"))

    (error-translator-with-error-in-the-translator
            "ERROR"
            (let/cc return
                (execute-with-error-translator
                    (lambda ex ( return "ERROR"))
                    (lambda()
                        (execute-with-error-translator
                            (lambda ex undefined)
                            (make-thunk undefined2))))))

    ; This test shows why we cannot use an
    ; actual stack of error handlers -
    ; the error handler may or may not
    ; invoke an escaping continuation.
    (error-translator-can-just-evaluate
        "ERROR"
        (execute-with-error-translator
            (lambda ex "ERROR")
            (make-thunk undefined)))

    (error-translator-gives-continuation
        6
        (execute-with-error-translator
        (lambda (ex c) (c 6))
        (make-thunk undefined)))

    (stack-trace
        6
        (begin
            (define (top)
                (let/cc return
                    (begin
                        (define (bottom)
                            (let/cc c (return c)))
                        (define (next)
                            (+ (bottom) 1))
                        (- (next) 2))))
            (length (stack-trace (top)))))
)
