; Unit tests for call/cc
(simplestCallCC 5
    (call/cc (lambda (c)
            (+ 3 (c 5)))))
