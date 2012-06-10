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

; Now we'll try something more ambitious: implement
; the "amb" operator. Based on
; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
'(ambTest (4 3 5)
    (with (a (amb '(1 2 3 4 5 6 7))
           b (amb '(1 2 3 4 5 6 7))
           c (amb '(1 2 3 4 5 6 7)))

        (begin
            (assert (eq? (* c c) (+ (* a a) (* b b))))
            (assert (<  b a))
            (log (list a b c)))))
