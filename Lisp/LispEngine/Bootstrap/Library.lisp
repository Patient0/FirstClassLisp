; Y combinator allows us to write recursive code without
; mutating the environment.
(define Y 
    (lambda (m)
       (let z (lambda (f) (m (lambda (a) ((f f) a))))
	     (z z))))
; We could use the Y combinator here, but because we are defining
; 'length' using a define form, we can just recurse directly
(define (length list)
    (define length-tail
        ; Here, we make use of the "pattern matching" in lambda
        (lambda (so-far ()) so-far
                (so-far (x . y))
                    (length-tail (+ 1 so-far) y)))
    (length-tail 0 list))

(define (reverse l)
    (define reverse-tail
        (lambda (so-far ()) so-far
                (so-far (x . y))
                    (reverse-tail (cons x so-far) y)))
    (reverse-tail '() l))

(define (fold-right op initial xs)
    (define fold-right-tail
        (lambda (so-far ()) so-far
                (so-far (x . y)) (fold-right-tail (op x so-far) y)))
    (fold-right-tail initial (reverse xs)))

; Macro for constructing a fold operation.
(define-macro fold-loop (var values so-far initial . body)
    `(,fold-right (,lambda (,var ,so-far) (,begin ,@body)) ,initial ,values))

; mapcar can be defined in terms of fold
(define (mapcar f list)
    (fold-loop x list
               l ()
            (cons (f x) l)))

(define (map f . ll)
    (define map-tail
        (lambda
            (f so-far (() . rest))
                so-far
            (f so-far ll)
                (map-tail f (cons (apply f (mapcar car ll)) so-far)
                            (mapcar cdr ll))))
    (reverse (map-tail f '() ll)))

(define-macro loop (var values . body)
    `(,reverse (,mapcar (,lambda (,var) (,begin ,@body)) (reverse ,values))))

; Explicit currying. It might be nicer to
; have implicit currying - but need to think
; about how best to implement first.
(define (curry fn . args)
    (lambda x
        (apply fn (append args x))))

(define (macro? f)
    (not (nil? (unmacro f))))

(define (identity x) x)

(define (compose2 f g)
    (lambda (x)
        (f (g x))))
(define (compose . fns)
     (fold-right compose2 identity fns))

(define cadr (compose2 car cdr))
(define caddr (compose2 cadr cdr))
(define cdddr (compose cdr cdr cdr))

(define (make-find comparator)
    (define (find item list)
        (if (nil? list) list
        (comparator item (car list)) list
        (find item (cdr list))))
    find)

(define find (make-find eq?))

; Find the first element which matches
; a given predicate
(define (search predicate list)
    (define loop
            (lambda ((head . tail))
                        (if (predicate head)
                            head
                            (loop tail))
                    _ ()))
    (loop list))

(define (after pivot list)
    (match (find pivot list)
           () ()
           (p . tail) tail))

; Not efficient - but
; it will do for now
(define (before pivot list)
    (reverse (after pivot (reverse list))))


(define-macro make-thunk args
    `(,lambda ()
        (,begin ,@args)))

(define (force thunk)
    (thunk))

(define-macro try clauses
    (with* (body (before 'catch clauses)
           (ex-symbols . error-handler) (after 'catch clauses)
           c-symbol (gensym))
        `(,let-cc ,c-symbol
            (,execute-with-error-translator
                (,lambda ,ex-symbols (,c-symbol (,begin ,@error-handler)))
                (,make-thunk ,@body)))))

; A hygienic 'amb' macro.
;
; Using a factory function allows us to maintain
; the hygiene of 'amb-fail'.
; Use 'make-amb' to make an
; amb macro that you know won't interfere
; with anyone else's amb macro.

; The ability to do this - return a hygienically
; scoped 'amb' macro from a
; function, is something that is only possible
; in a Lisp with First Class macros and continuations
; (i.e. First Class Lisp!)
(define (make-amb-macro exhausted)
    (define amb-fail exhausted)
    (define (set-fail thunk)
        (set! amb-fail thunk))
    (define (get-fail)
        amb-fail)

    ; Based on
    ; http://c2.com/cgi/wiki/?AmbSpecialForm
    (define amb
        (define expand-amb (lambda
            ()  (list force (get-fail))
            (x) x
            (x y)
                (with* (old-fail (gensym)
                        c (gensym))
                `(,let ,old-fail (,get-fail)
                    (,force
                        (,let-cc ,c
                            (,set-fail
                                (,make-thunk
                                    (,set-fail ,old-fail)
                                    (,c (,make-thunk ,y))))
                            (,make-thunk ,x)))))
            (x . rest)
                `(,amb ,x (,amb ,@rest))))
     (macro expand-amb))
    amb)


; This 'amb' is adapted from
; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
; It's most useful when you have an already evaluated list
; of possibilities
(define (make-amb-function exhausted)
    (define (current-continuation) 
      (call-cc
       (lambda (cc)
         (cc cc))))
    (define fail-stack ())
    (define (fail)
        (match fail-stack
            (back-track-point . rest)
                (begin
                    (set! fail-stack rest)
                    (back-track-point back-track-point))
            _
                (exhausted)))
    (lambda
        ; So that 'assert' can be the same for both forms
        ()
            (fail)
        (choices)
            (let cc (current-continuation)
                (match choices
                    () (fail)
                    (choice . remaining-choices)
                        (begin
                            (set! choices remaining-choices)
                            (set! fail-stack (cons cc fail-stack))
                            choice)))))

; Given an amb macro, make an appropriate
; assert function. Once again, this sort of
; thing is only possible with first-class macros.
(define (make-assert amb)
    (lambda
        (#f) (amb)
        (condition) #t))

; Flatten takes a list of lists and produces a single list
; e.g.
; > (flatten '((1 2) (3 4)))
; (1 2 3 4)
(define flatten (curry apply append))
; Stitch takes a list of tuples and an element
; and returns another list
; with that element stitched on to each of the tuples:
; e.g.
; > (stitch '(1 2 3) 4)
; ((4 . 1) (4 . 2) (4 . 3))
(define (stitch tuples element)
    (mapcar (curry cons element) tuples))
; Cartesian takes two lists and returns their
; cartesian product
(define (cartesian l1 l2)
    (flatten (mapcar (curry stitch l2) l1)))
; cartesian-lists takes a list of lists
; and returns a single list containing the cartesian product of all of the lists.
; We start with a list containing a single 'nil', so that we create a
; "list of lists" rather than a list of "tuples".
(define cartesian-lists (curry fold-right cartesian '(())))
; cartesian-map takes a n-argument function and n lists
; and returns a single list containing the result of calling that
; n-argument function for each combination of elements in the list:
; > (cartesian-map list '(a b) '(c d e) '(f g))
; ((a c f) (a c g) (a d f) (a d g) (a e f) (a e g) (b c f)
;  (b c g) (b d f) (b d g) (b e f) (b e g))
(define (cartesian-map f . lists)
    (map (curry apply f) (cartesian-lists lists)))

; filter can be thought of as a fold-right
; in which the 'join' function is either
; 'cons' or just the right list depending
; on the current value.
(define (filter predicate list)
    (define (join x so-far)
            (if (predicate x)
                (cons x so-far)
                so-far))
    (fold-right join '() list))

(define (make-remove comparator)
    (lambda (x list)
        (let predicate
                (lambda (y) (not (comparator x y)))
            (filter predicate list))))

(define (make-in comparator)
    (define find (make-find comparator))
    (lambda (x list)
        (if (nil? (find x list)) #f #t)))

(define in (make-in equal?))

(define (replace old-value new-value list)
    (mapcar (lambda (x)
                (if (eq? old-value x) new-value x)) list))

(define (make-assoc comparator)
    (define (finder key list)
            (match list
                () #f
                (pair . rest)
                    (if (comparator key (car pair))
                        pair
                        (finder key rest))))
    finder)

(define assoc (make-assoc equal?))

; Remove all instances of 'x' from a list
(define remove (make-remove equal?))

; Optimized for just removing one element
; from a list which is a set (so we don't
; care about ordering)... and this is only
; an optimization in that 'append'
; is a builtin. Was it worth it?
(define (remove-one x list)
    (define remove-one-tail
        (lambda (so-far ())
                    so-far
                (so-far (head . tail))
                    (if (equal? x head)
                        (append so-far tail)
                        (remove-one-tail (cons head so-far) tail))))
    (remove-one-tail '() list))

; For sudoku solver, we'll define a 'dictionary'
; concept - but implement it very inefficiently
; as just a list of pairs
(define make-dict identity)

; For now, no error handling: if the key
; doesn't exist it's an error!
(define (lookup dict key)
    (cdr (assoc key dict)))

(define (dict-update d key new-value)
    (define new-pair (cons key new-value))
    (define dict-tail
            (lambda (so-far ())
                        so-far
                    (so-far (pair . tail))
                        (if (equal? (car pair) key); found it!
                                (append so-far (cons new-pair tail))
                            (dict-tail (cons pair so-far) tail))))
    (dict-tail () d))

; Using quick-sort as the algorithm here.
; It's not even a stable sort - but it'll work
; for now.
(define (make-sorter comparator)
    (define sort
        (lambda (()) ()
                ((pivot . rest))
                    (append (sort (filter (compose2 not (curry comparator pivot)) rest))
                            (list pivot)
                            (sort (filter (curry comparator pivot) rest)))))
    sort)
    
(define sort (make-sorter <))

; For now, our 'set' constructor will just
; create and compare using equality predicate.
; i.e. an O(n^2) algorithm.
(define (make-unique comparator)
    (define (join x so-far)
        (if (in x so-far)
                so-far
            (cons x so-far)))
    (curry fold-right join '()))
        
(define unique (make-unique equal?))

(define (repeat fn count)
    (define loop
            (lambda (0 so-far) so-far
                    (n so-far)
                        (let next (- n 1)
                        (loop next (cons (fn next) so-far)))))
    (loop count '()))

(define-macro index-loop (var count . body)
    `(,repeat (,lambda (,var) (,begin ,@body)) ,count))

(define (max (first . rest) . less)
    (let less (if (nil? less) < less)
        (fold-loop x rest
                   max-so-far first
                   (if (less max-so-far x)
                            x
                        max-so-far))))

; Macro for looping subject to some condition
(define-macro filter-loop (var list predicate)
    `(,filter (,lambda (,var) ,predicate) ,list))

(define or (macro
        (lambda
            () #f
            (first . rest)
                (let temp (gensym)
                    `(,let ,temp ,first
                        (,if ,temp ,temp (,or ,@rest)))))))

(define and (macro
        (lambda
            () #t
            (condition)
                condition
            (first . rest)
                `(,if ,first (,and ,@rest) #f))))
