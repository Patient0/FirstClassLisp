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
)
(tests
    (squares-length
        81
        (length squares))

    (unitlist
        27
        (length unitlist))
)
