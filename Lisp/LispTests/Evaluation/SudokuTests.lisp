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
                            
)
(tests
    (squares-length
        81
        (length squares))

    (unitlist
        27
        (length unitlist))

    (units
        (((A . 1) (A . 2) (A . 3) (B . 1) (B . 2) (B . 3) (C . 1) (C . 2) (C . 3))
         ((A . 2) (B . 2) (C . 2) (D . 2) (E . 2) (F . 2) (G . 2) (H . 2) (I . 2))
         ((C . 1) (C . 2) (C . 3) (C . 4) (C . 5) (C . 6) (C . 7) (C . 8) (C . 9)))
        (lookup units '(C . 2)))
)
