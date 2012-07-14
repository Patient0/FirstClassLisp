; Unit tests for call/cc
(setup
    (define amb (make-amb throw))
    (define assert (make-assert amb)))
(tests
    (simplestCallCC 5
        (call/cc (lambda (c)
                (+ 3 (c 5)))))
    (noopCallCC 5
        ((call/cc call/cc) (lambda (x) 5)))

    (backwardsCC 5
        ((call/cc (lambda (k) k)) (lambda (x) 5)))

    (letcc 23
        (let/cc c (+ 3 (c 23))))

    (letcc-has-implicit-begin
        6
        (begin
            (define x 5)
            (let/cc c
                (set! x 6)
                c x)))
)
