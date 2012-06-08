; Unit tests for call/cc
(simplestCallCC 5
    (call/cc (lambda (c)
            (+ 3 (c 5)))))
(noopCallCC 5
    ((call/cc call/cc) (lambda (x) 5)))
(backwardsCC 5
    ((call/cc (lambda (k) k)) (lambda (x) 5)))
