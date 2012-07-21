(define digits '(1 2 3 4 5 6 7 8 9))
; (define rows '(A B C D E F G H I))
; Use digits for rows too
(define rows digits)
(define cols digits)
(define cross cartesian)
(define squares (cartesian rows cols))
(define unitlist
    (append
        (cartesian-map cross
            '((1 2 3) (4 5 6) (7 8 9))
            '((1 2 3) (4 5 6) (7 8 9)))
        (loop c cols (cross rows (list c)))
        (loop r rows (cross (list r) cols))))

(define (units-for-square s)
    (filter-loop unit unitlist (in s unit)))

(define units
    (make-dict (loop s squares
                   (cons s (units-for-square s)))))

(define (peers-for-square s)
    (remove-one s (unique (flatten (lookup units s)))))

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
    (let values (filter-loop c (string->list grid)
                    (or (in c digits) (in c '(0 dot))))
        (if (eq? (length values) 81)
            (zip squares values)
            (throw "Could not parse grid"))))

(define eliminate-peers
    (lambda
        (values s (d))
            (fold-loop peer (lookup peers s)
                       v values
                        (eliminate v peer d))
    ; More than one remaining - leave unchanged
        (values . _) values))

(define amb (make-amb-function (curry throw "No solutions found")))

(define (fail)
    (amb))

; After removing d from s and its peers,
; does d now only appear in one place for the units
; of s? If so, "assign" to that place.
(define (check-units values s d)
    (fold-loop u (lookup units s)
               v values
        (let dplaces (filter-loop s u (in d (lookup v s)))
             (match dplaces
                () (fail)
                ; d only appears in 's' in this unit
                ; So assign d so square s in values
                (s) (assign s d v)
                ; do nothing.
                _   v))))

(define (remove-digit d digits)
    (match (remove-one d digits)
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
    (fold-loop eliminated (remove-one d (lookup values s))
               v values
        (eliminate v s eliminated)))

(define all-but-one (remove 1 digits))

(define (square-to-try values)
    (let/cc return
        (loop attempt all-but-one
            (loop entry values
                (if (eq? (length (cdr entry)) attempt)
                    (return entry)
                    nil)))))
           
; True if each square in values has one value
(define solved?
    ; Match against the inputs.
    ; If there's a square with 1 value, then it's
    ; solved so long as the rest also satisfy this
    (lambda (((s . (x)) . rest))
                (solved? rest)
    ; We've found a square with more than one value.
    ; It's not solved.
            (((s . _) . rest))
                #f
    ; No more squares to check. Must be solved
            (())
                #t))

(define (solve values)
    (if (solved? values)
        values
        (with* ((s . possible) (square-to-try values)
                ; We have several possible values for square 's'.
                ; Tell 'amb' to magically pick the "right one" for
                ; us and assign it to s.
                d (amb possible))
                (write-line "Assigning {0} to square {1}" d s)
                (solve (assign s d values)))))

(define empty-grid (make-dict (loop s squares
                                (cons s digits))))

(define (parse-grid grid)
    (fold-loop (s . d) (grid->values grid)
               values empty-grid
            (if (in d digits)
                (assign s d values)
                values)))

(define (values->list values)
    (loop r rows
        (loop c cols
            (lookup values (cons r c)))))

(define (display-grid values)
    (loop r (values->list values)
        (write-line "{0}" r))
    nil)

;(write-line "Solving grid1...")
;(define parsed1 (parse-grid grid1))
;(display-grid parsed1)
;(write-line "Solving grid2...")
;(define parsed2 (parse-grid grid2))
;(write-line "With wrote deduction we only get to:")
;(display-grid parsed2)
;(write-line "Solving using non-deterministic search...")
;(define solution2 (solve parsed2))
;(write-line "Solution:")
;(display-grid solution2)
