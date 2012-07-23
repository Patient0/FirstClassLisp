(setup
    (run "..\\..\\..\\Lisp\\Examples\\Sudoku.lisp" (env))
)
(tests
    (show-digits
        (1 2 3 4 5 6 7 8 9)
        (show-digits all-digits))

    (remove-digit
        (1 3 4)
        (show-digits (remove-digit (digit-set 1 2 3 4) 2)))

    (remove-digit-already-gone
        (1 2 3 4)
        (show-digits (remove-digit (digit-set 1 2 3 4) 5)))

    (add-digit
        (1 3 4)
        (show-digits (add-digit (digit-set 1 4) 3)))

    (solved-digit?
        (#f 6)
        (mapcar solved-digit? (list (digit-set 2 4) (digit-set 6))))

    (grid-get-set
        23
        (begin
            (define g (new-grid))
            (set-square! g 45 23)
            (get-square g 45)))

    (solved-true?
        #t
        (solved? parsed1))

    (solved-false?
        #f
        (solved? parsed2))

    ; We can solve grid1 using only technique 1:
    ; eliminate peers.
    (parse-grid
        (((4) (8) (3) (9) (2) (1) (6) (5) (7))
         ((9) (6) (7) (3) (4) (5) (8) (2) (1))
         ((2) (5) (1) (8) (7) (6) (4) (9) (3))
         ((5) (4) (8) (1) (3) (2) (9) (7) (6))
         ((7) (2) (9) (5) (6) (4) (1) (3) (8))
         ((1) (3) (6) (7) (9) (8) (2) (4) (5))
         ((3) (7) (2) (6) (8) (9) (5) (1) (4))
         ((8) (1) (4) (2) (5) (3) (7) (6) (9))
         ((6) (9) (5) (4) (1) (7) (3) (8) (2)))
        (grid->lists parsed1))

    ; Grid2 requires search
    (solve-grid2
        (((4) (1) (7) (3) (6) (9) (8) (2) (5))
         ((6) (3) (2) (1) (5) (8) (9) (4) (7))
         ((9) (5) (8) (7) (2) (4) (3) (1) (6))
         ((8) (2) (5) (4) (3) (7) (1) (6) (9))
         ((7) (9) (1) (5) (8) (6) (4) (3) (2))
         ((3) (4) (6) (9) (1) (2) (7) (5) (8))
         ((2) (8) (9) (6) (4) (3) (5) (7) (1))
         ((5) (7) (3) (2) (9) (1) (6) (8) (4))
         ((1) (6) (4) (8) (7) (5) (2) (9) (3)))
        (grid->lists solution2))
)
