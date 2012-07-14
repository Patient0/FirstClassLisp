(tests
    (define-macro 8
        (begin
            ; Just duplicate the original 'let' macro
            (define-macro slet (var value body)
                    `((,lambda (,var) ,body) ,value))
            (slet x 5 (+ x 3))))

    ; Expand gives useful way to debug macro expansions
    (expand
        ((lambda-symbol (x) (+ x 3)) 5)
        (begin
            ; So that the unit test can 'work'
            (define lambda 'lambda-symbol)
            (define-macro slet (var value body)
                    `((,lambda (,var) ,body) ,value))
            (expand (env) '(slet x 5 (+ x 3)))))

    ; For a non-macro, we'll have expand just
    ; return the original expression
    (expand-non-macro
        (square 3)
        (begin
            (define (square x)
                (* x x))
            (expand (env) '(square 3))))
        
    ; We ought to support the traditional scheme
    ; "define function" syntax
    (define-function 25
        (begin
            (define (square x) (* x x))
            (square 5)))

    (define-function-two-args 18
        (begin
            (define (subtract x y) (- x y))
            (subtract 21 3)))

    (define-function-multiple-sub-expressions 10
        (begin
            (define (subtract x y)
                35 ; this is evaluated but ignored
                (- x y))
            (subtract 15 5)))

    ; Our own homegrown 'list comprehension' syntax
    ; which is implemented in terms of map
    (loop (1 4 9 16)
        (loop x '(1 2 3 4)
            (* x x)))

    ; 'with' is like let but allows multiple variable
    ; definitions.
    (with (5 100 7)
        (with (x 5 y 100 z 7)
            (list x y z)))

    (let/cc 23
        (let/cc return
            (+ 5 (return 23))))

    (match 3
        (with (x 4 y 5)
            (match `(,x ,y)
                (1 2) 0
                (2 3) 1
                (3 4) 2
                (4 z) (- z 2)
                (5 6) 4)))
)
