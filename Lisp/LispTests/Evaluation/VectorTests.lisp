(tests
    (make-vector
        #(0 0 0)
        (make-vector 3))

    (make-vector
        #("X" "X" "X")
        (make-vector 3 "X"))

    (vector-length
        3
        (vector-length '#(0 0 0)))

    ; Hmm: could we do pattern
    ; matching for vector?
    (vector?
        (#t #f)
        (mapcar vector? '(#(0) 5)))

    (vector
        #(1 2 3)
        (vector 1 2 3))

    (vector-ref
        2
        (vector-ref #(1 2 3) 1))

    (vector-set!
        #(1 4 3)
        (begin
            (define x #(1 2 3))
            (vector-set! x 1 4)
            x))

    (vector-copy
        #(1 2 3)
        (begin
            (define x #(1 2 3))
            (define y (vector-copy x))
            (vector-set! x 1 3)
            y))
)
