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

(define eliminate-peers
    (lambda
        (values s (d))
            (let join (lambda (p values)
                    (eliminate values p d))
            (fold-right join values (lookup peers s)))
    ; More than one remaining - leave unchanged
        (values . _) values))

(define amb (make-amb throw))

; apply does not work for macros - so this
; helper function bridges the gap
(define (amb-list possible)
    (eval `(,amb ,@possible) (env)))

(define (fail)
    (amb))

; After removing d from s and its peers,
; does d now only appear in one place for the units
; of s? If so, "assign" to that place.
; TODO: We need some more macros to simplify these
; anonymous lambda expressions!
(define (check-units values s d)
    (define (add-unit u values)
        (with* (d-is-in-square
                (lambda (s) (in d (lookup values s)))
               dplaces (filter d-is-in-square u))
              (match dplaces
                () (fail)
                ; d only appears in 's' in this unit
                (s) (assign s d values)
                ; do nothing.
                _   values)))
    (fold-right add-unit values (lookup units s)))

(define (remove-digit d digits)
    (match (remove d digits)
            () (fail)
            possible possible))

; Eliminate d from the list of possible values
; for square s
(define (eliminate values s d)
    (define current (lookup values s))
    (if (in d current)
            (with* (possible (remove-digit d current)
                   values (dict-update values s possible)
                   values (eliminate-peers values s possible)
                   values (check-units values s d))
                  values)
        values))

; Return the 'values' that results from
; assigning d to square s
(define (assign s d values)
    (define others (remove d (lookup values s)))
    (define (join d values)
        (eliminate values s d))
    (fold-right join values others))

(define all-but-one (remove 1 digits))

(define (square-to-try values)
    (let/cc return
        (loop attempt all-but-one
            (loop entry values
                (if (eq? (length (cdr entry)) attempt)
                    (return entry)
                    nil)))))
           
(define (solved? values)
    (let/cc return
        (loop (s . possible) values
            (match possible
                (d) nil
                (d . x) (return #f)
                _ (fail)))
        #t))

(define (solve values)
    (if (solved? values)
        values
        (with* ((s . possible) (square-to-try values)
               d (amb-list possible))
                (write-line "Assigning {0} to square {1}" d s)
                (solve (assign s d values)))))

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
    (loop r (values->list values)
        (write-line "{0}" r))
    nil)

(write-line "Solving grid1...")
(define parsed1 (parse-grid grid1))
(display-grid parsed1)
(write-line "Solving grid2...")
(define parsed2 (parse-grid grid2))
(write-line "With wrote deduction we only get to:")
(display-grid parsed2)
(write-line "Solving using non-deterministic search...")
(define solution2 (solve parsed2))
(write-line "Solution:")
(display-grid solution2)
