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

; Eliminate d from the list of possible values
; for square s
(define (eliminate values s d)
    (dict-update values s (remove d (lookup values s))))

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

(define (display-grid values)
    (loop r rows
        (write-line "{0}"
            (loop c cols
                (lookup values (cons r c)))))
    nil)

(define parsed (parse-grid grid1))
(display-grid parsed)
