; Now we'll try something more ambitious: implement
; the "amb" operator. Based on
; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
; The amb operator lets you 'assign' values to a set of values,
; then execute based on those values. Any paths which subsequently
; fail due to an assert backtrack to try other values instead.
; Hmmm. My problem with this example is that you could implement
; the same thing with a nested loop and simple 'yield return.
; A more sophisticated example is called for...
(define error (lambda () "amb tree exhausted"))
(define (current-continuation) 
  (call/cc
   (lambda (cc)
     (cc cc))))

; Adapted from
; http://matt.might.net/articles/programming-with-continuations--exceptions-backtracking-search-threads-generators-coroutines/
;
; Matt Might's implementation of amb
; uses explicit *global* state, which makes me fell queasy:
; what if you wanted to solve lots of different problems
; in parallel threads?
; 
; In order to let the user keep control of the state,
; keep the state inside an 'amb-environment'. The
; amb-environment has to be constructed with
; an error function which will be called if
; no solutions exist.
(define (make-amb-environment error)
    ; fail-stack : list[continuation]
    (define fail-stack ())
    (define (fail)
        (match fail-stack
            (back-track-point . rest)
                (begin
                    (set! fail-stack rest)
                    (back-track-point back-track-point))
            _
                (error)))
    (lambda
        ('fail) fail
        ('amb choices)
            (let cc (current-continuation)
                (match choices
                    () (fail)
                    (choice . remaining-choices)
                        (begin
                            (set! choices remaining-choices)
                            (set! fail-stack (cons cc fail-stack))
                            choice)))

        ('assert #t) #t
        ('assert _) (fail)))

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
