; The archetypical fibonacci program...
(define (fib n)
    (match n
           0 0
           1 1
           n (+ (fib (- n 1)) (fib (- n 2)))))
(display (fib 8))
