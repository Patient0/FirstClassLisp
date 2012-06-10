; Unit tests for call/cc
(simplestCallCC 5
    (call/cc (lambda (c)
            (+ 3 (c 5)))))
(noopCallCC 5
    ((call/cc call/cc) (lambda (x) 5)))

(backwardsCC 5
    ((call/cc (lambda (k) k)) (lambda (x) 5)))

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
; Now we'll try something more ambitious: implement
; the "amb" operator. Based on
; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
;(amb (4 3 5)
;    (with (a (amb '(1 2 3 4 5 6 7))
;           b (amb '(1 2 3 4 5 6 7))
;           c (amb '(1 2 3 4 5 6 7)))
;
;        (begin
;            (assert (= (* c c) (+ (* a a) (* b b))))
;            (assert (<  b a)))))
