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

    ; Now we'll try something more ambitious: implement
    ; the "amb" operator. Based on
    ; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
    (ambTest (4 3 5)
        (begin
            ; '404' for no solutions found. Haven't implemented
            ; string literals so can't report anything else!!
            (define error (lambda () 404))
            (define env (make-amb-environment error))
            (with (a (env 'amb '(1 2 3 4 5 6 7))
                   b (env 'amb '(1 2 3 4 5 6 7))
                   c (env 'amb '(1 2 3 4 5 6 7)))

                (begin
                    ; We only want pythagorean triples
                    (env 'assert (eq? (* c c) (+ (* a a) (* b b))))
                    ; And only those with the second value less
                    ; than the first.
                    (env 'assert (<  b a))
                    (log (list a b c)))))))
