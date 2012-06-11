; Now we'll try something more ambitious: implement
; the "amb" operator. Based on
; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
; The amb operator lets you 'assign' values to a set of values,
; then execute based on those values. Any paths which subsequently
; fail due to an assert backtrack to try other values instead.
; Hmmm. My problem with this example is that you could implement
; the same thing with a nested loop and simple 'yield return.
; A more sophisticated example is called for...
(define error (lambda () 404))
(define env (make-amb-environment error))
(with (a (env 'amb '(1 2 3 4 5 6 7))
       b (env 'amb '(1 2 3 4 5 6 7))
       c (env 'amb '(1 2 3 4 5 6 7)))

    (begin
        ; We only want pythagorean triples
        (env 'assert (eq? (* c c) (+ (* a a) (* b b))))
        (display (list a b c))
        ; And only those with the second value less
        ; than the first.
        (env 'assert (<  b a))
        (display (list a b c))))
