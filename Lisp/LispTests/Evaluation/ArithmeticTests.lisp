; Simple arithmetic tests
(tests
    (add 5
        (+ 2 3))
    (subtract 3
        (- 7 4))
    (times 20
        (* 5 4))
    (divide 5
        (/ 20 4))
    ; Test looping
    (length
        3
        (length '(2 4 6))))
