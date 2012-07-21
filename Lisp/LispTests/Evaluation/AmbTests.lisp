(setup
    (define amb (make-amb-macro throw))
    (define assert (make-assert amb)))
(tests

    ; This example came from
    ; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
    (ambTest-Pythagorean (4 3 5)
        (begin
            (with (a (amb 1 2 3 4 5 6 7)
                   b (amb 1 2 3 4 5 6 7)
                   c (amb 1 2 3 4 5 6 7))
                    ; We only want pythagorean triples
                    (assert (eq? (* c c) (+ (* a a) (* b b))))
                    ; And only those with the second value less
                    ; than the first.
                    (assert (<  b a))
                    (list a b c))))

    ; Inspired by
    ; http://mitpress.mit.edu/sicp/full-text/sicp/book/node89.html
    ; amb has to be a macro to allow (possibly infinitely) expensive
    ; clauses to be amongst those included. Here, 'anything-starting-from'
    ; is an infinite combination.
    (amb-is-lazy
        6
        (begin
            (define (anything-starting-from n)
                (amb n (anything-starting-from (+ n 1))))
            (let a (anything-starting-from 3)
                (assert (eq? a 6))
                a)))

    ; Here, we demonstrate that if you make
    ; multiple 'amb' operators then they do not
    ; interact with each other, which is desirable
    ; if only for performance. It's also desirable
    ; in that we usually only want small
    ; parts of the program to execute non-deterministically
    ; rather than the entire program.
    (multiple-amb
        "exhausted"
        (try
            (with* (exhausted (curry throw "exhausted")
                    amb1 (make-amb-macro exhausted)
                    assert1 (make-assert amb1)
                    amb2 (make-amb-macro exhausted)
                    assert2 (make-assert amb2))
                (define a (amb1 1 2))
                (define b (amb2 4 8))
                (assert1 (eq? 9 (+ a b)))
                (list a b))
         catch (msg c)
            msg))


    (ambFunctionTest-Pythagorean (4 3 5)
        (begin
            (define amb (make-amb-function (curry throw "exhausted")))
            (define assert (make-assert amb))
            (with (a (amb '(1 2 3 4 5 6 7))
                   b (amb '(1 2 3 4 5 6 7))
                   c (amb '(1 2 3 4 5 6 7)))
                    ; We only want pythagorean triples
                    (assert (eq? (* c c) (+ (* a a) (* b b))))
                    ; And only those with the second value less
                    ; than the first.
                    (assert (<  b a))
                    (list a b c))))
)
