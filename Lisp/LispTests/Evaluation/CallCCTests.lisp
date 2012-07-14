; Unit tests for call/cc
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

    ; Test the 'amb' operator that's been implemented as
    ; a builtin.
    ; the "amb" operator.
    (ambTest-Pythagorean (4 3 5)
        (begin
            ; '404' for no solutions found. Haven't implemented
            ; string literals so can't report anything else!!
            (with (a (amb 1 2 3 4 5 6 7)
                   b (amb 1 2 3 4 5 6 7)
                   c (amb 1 2 3 4 5 6 7))

                (begin
                    ; We only want pythagorean triples
                    (assert (eq? (* c c) (+ (* a a) (* b b))))
                    ; And only those with the second value less
                    ; than the first.
                    (assert (<  b a))
                    (list a b c))))))
