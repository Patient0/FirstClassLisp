; Tests of library functions written in FCLisp itself.
; The implementations are defined in Builtins.lisp
(tests
    (foldrSimplest 10
        (fold-right + 10 '()))
    (foldrTest 25
        (fold-right + 10 '(1 2 3 4 5)))

    (mapCarSimplest ()
        (mapcar (lambda (x) (* x x)) '()))
    (mapCarTest (1 4 9)
        (mapcar (lambda (x) (* x x)) '(1 2 3)))

    (mapSquare (1 4 9)
        (map (lambda (x) (* x x)) '(1 2 3)))
    (mapTest (4 10 18)
        (map * '(1 2 3) '(4 5 6)))
    (mapThreeTest ((1 4 7) (2 5 8) (3 6 9))
     (map list '(1 2 3) '(4 5 6) '(7 8 9)))

    ; We've adopted explicit currying... it might
    ; be nicer to have implicit currying. Need to
    ; think about the best way to implement though.
    (curry 5
        (let add1 (curry + 1)
            (add1 4)))

    (fold-right-folds-right
        (1 2 3)
        (fold-right cons '() '(1 2 3)))

    (compose2
           5
           ((compose2 car cdr) '(1 5 12)))
    (composeN
           12
           ((compose car cdr cdr) '(1 5 12)))

    (cadr 5 (cadr '(1 5 12)))
    (caddr 12 (caddr '(1 5 12)))
    (cdddr (17) (cdddr '(1 5 12 17)))

    (find (5 6 7)
        (find 5 '(1 2 3 4 5 6 7)))

    (find-non-existent ()
        (find 3 '(1 2 4)))

    (find-first (1 2 3)
        (find 1 '(1 2 3)))

    (after (4 5)
        (after 3 '(1 2 3 4 5)))

    (before (1 2)
        (before 3 '(1 2 3 4 5)))

    (try-catch-fail
        "Undefined symbol \'undefined\'"
            (try
                undefined
                undefined-not-reach
             catch ex (car ex)))

    (try-catch-success
        "SUCCESS"
            (try
                "SUCCESS"
             catch ex
                (car ex)))

    ; We allow multiple statements after the catch
    (catch-has-implicit-begin
        "Undefined symbol \'undefined\'"
            (try
                undefined
            catch ex
                (log (* 3 5))
                (car ex)))

    ; We'll use 'throw' as the builtin
    ; for raising errors
    (throw
        "This is an error message"
        (try
            (throw "This is an error message")
        catch (msg c)
            msg))

    (thunk-simplest
        5
        (force (make-thunk 5)))

    (thunk-does-not-evaluate
        5
        (begin
            (make-thunk undefined)
            5))

    (make-thunk-has-implicit-begin
        6
        (force
            (make-thunk 5 6)))

    (loop-has-implicit-begin
        (1 2 3)
        (loop x '(1 2 3)
            5
            x))

    (loop-evaluates-left-to-right
        (3 2 1)
        (begin
            (define x nil)
            (loop y '(1 2 3)
                (set! x (cons y x)))
            x))

    (cartesian-map
        ((1 4) (1 8) (2 4) (2 8))
        (cartesian-map list '(1 2) '(4 8)))
)
