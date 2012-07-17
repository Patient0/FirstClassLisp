; Based on http://norvig.com/sudoku.html
; The performance is currently atrocious:
; 1.6 million steps to perform the initialization!
; Yet: All this happens in under 3 seconds on my PC!
(setup
    (ref mscorlib)
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
                        
    (define grid1 "003020600900305001001806400008102900700000008006708200002609500800203009005010300")
    (define grid2 "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......")
    (define hard  ".....6....59.....82....8....45........3........6..3.54...325..6..................")

    (define (string->list s)
        (define (convert char)
            (let schar (.ToString char)
                (if (System.Char.IsDigit char)
                        (System.Convert.ToInt32 schar)
                    (eq? "." schar)
                        'dot
                (string->symbol schar))))
        (with (array (.ToCharArray s)
               length (.get_Length s))
            (repeat (lambda (i)
                        (convert (.GetValue array i))) length)))

    (define zip (curry map cons))
    (define (grid->values grid)
        (let values (filter
            (lambda (c)
                (if (in c digits) #t
                    (in c '(0 dot)) #t
                    #f))
            (string->list grid))
            (if (eq? (length values) 81)
                (zip squares values)
                (throw "Could not parse grid"))))

    (define (eliminate-peers values s d)
        (define (join p values)
                (eliminate values p d))
        (fold-right join values (lookup peers s)))

    ; Eliminate d from the list of possible values
    ; for square s
    (define (eliminate values s d)
        (define current (lookup values s))
        (if (not (in d current))
            values
            (with (possible (remove d current)
                   values (dict-update values s possible))
                  (match possible
                       (last) (eliminate-peers values s last)
                       _ values))))

    ; Return the 'values' that results from
    ; assigning d to square s
    (define (assign s d values)
        (define others (remove d (lookup values s)))
        (define (join d values)
            (eliminate values s d))
        (fold-right join values others))

    (define empty-grid (make-dict (loop s squares
                                    (cons s digits))))

    (define (parse-grid grid)
        (define (join (s . d) values)
                (if (in d digits)
                    (assign s d values)
                    values))
        (fold-right join empty-grid (grid->values grid)))

    (define (values->list values)
        (loop r rows
            (loop c cols
                (lookup values (cons r c)))))

    (define (display-grid values)
        (loop r rows
            (write-line "{0}"
                (loop c cols
                    (lookup values (cons r c)))))
        nil)
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

    (string->list
        (1 2 3 4 A B C D)
        (string->list "1234ABCD"))

    (grid->values
        (81 81 81)
        (map (compose length grid->values)
            (list grid1 grid2 hard)))

    (grid1-easy
        (((4) (8) (3) (9) (2) (1) (6) (5) (7))
         ((9) (6) (7) (3) (4) (5) (8) (2) (1))
         ((2) (5) (1) (8) (7) (6) (4) (9) (3))
         ((5) (4) (8) (1) (3) (2) (9) (7) (6))
         ((7) (2) (9) (5) (6) (4) (1) (3) (8))
         ((1) (3) (6) (7) (9) (8) (2) (4) (5))
         ((3) (7) (2) (6) (8) (9) (5) (1) (4))
         ((8) (1) (4) (2) (5) (3) (7) (6) (9))
         ((6) (9) (5) (4) (1) (7) (3) (8) (2)))
        (values->list (parse-grid grid1)))
)
