; Based on http://norvig.com/sudoku.html
(setup
    (define digits '(1 2 3 4 5 6 7 8 9))
    (define rows '(A B C D E F G H I))
    (define cols digits)
    (define cross cartesian)
    (define squares (cartesian rows cols))
    (define unitlist
        (append
            (cartesian-map cross
                '((A B C) (D E F) (G H I))
                '((1 2 3) (4 5 6) (7 8 9)))
            (loop c cols (cross rows (list c)))
            (loop r rows (cross (list r) cols))))

    (define (units-for-square s)
        (filter (lambda (unit)
                    (in s unit))
                unitlist))

    (define units
        (make-dict (loop s squares
                       (cons s (units-for-square s)))))

    (define (peers-for-square s)
        (remove s (unique (flatten (lookup units s)))))

    (define peers
        (make-dict (loop s squares
                    (cons s (peers-for-square s)))))
                        
                            
)
(tests
    (squares-length
        81
        (length squares))

    (unitlist
        27
        (length unitlist))

    ; we're not currently sorting these guys - the order is
    ; somewhat arbitrary.
    ; Therefore, it's only robust to compare the length for now.
    (units
        27
        (length (flatten (lookup units '(C . 2)))))

    (peers
        20
        (length (lookup peers '(C . 2))))
)
