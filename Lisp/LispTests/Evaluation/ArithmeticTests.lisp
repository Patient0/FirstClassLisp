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

    ; bitwise functions. Used for bit sets.
    (bit-and 4
        (bit-and 4 7))

    (bit-or 7
        (bit-or 4 7))

    (bit-shift 14
        (bit-shift 7 1))

    ; Test looping
    (length
        3
        (length '(2 4 6))))
