(setup
    ; TODO Need to make this ref unnecessary
    (ref mscorlib)
    (define digits '(1 2 3 4 5 6 7 8 9))

    ; For efficiency, we'll use a single integer
    ; 'bit field' to represent which digits are
    ; available. 0 means a contradiction.
    ; We do this rather than implement
    ; lots of builtin string primitives, which
    ; would have been the thing to do if we'd
    ; followed Peter Norvig's Python program
    ; exactly.
    (define (sub1 x)
            (- x 1))

    (define (digit-bit digit)
            (bit-shift 1 (sub1 digit)))

    (define (add-digit ds d)
        (bit-or ds (digit-bit d)))

    (define solved-digit?
        (lambda (1) 1
                (2) 2
                (4) 3
                (8) 4
                (16) 5
                (32) 6
                (64) 7
                (128) 8
                (256) 9
                (_) #f))

    (define (digit-set . digits)
        (fold-loop d digits
                   ds 0
                   (add-digit ds d)))

    (define all-digits (apply digit-set digits))
    (define none (digit-set))

    (define zero? (curry eq? 0))
    (define none? zero?)
    (define not-zero? (compose2 not zero?))

    (define (remove-digit ds d)
        (bit-and ds (- all-digits (digit-bit d))))

    (define (has-digit? ds d)
        (not-zero? (bit-and (digit-bit d) ds)))

    ; Inverse function of digit-set constructor
    (define (show-digits ds)
        (filter-loop d digits
            (has-digit? ds d)))

    (define num-squares 81)

    ; Grid representation. Use a vector
    ; and row/column arithmetic.
    (define empty-grid
        (make-vector num-squares all-digits))

    (define (index (row . column))
            (+ (sub1 column) (* (sub1 row) 9)))

    (define square cons)

    (define (get-square grid s)
            (vector-ref grid (index s)))

    (define (set-square! grid s ds)
            (vector-set! grid (index s) ds))

    (define (get-digits grid s)
            (show-digits (get-square grid s)))

    (define copy-grid vector-copy)
    (define (new-grid)
        (copy-grid empty-grid))

    ; Use digits for rows and the columns
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

    (define (peers-for-square s)
        (remove s (flatten (get-square units s))))

    (define (make-grid square-function)
        (let g (make-vector num-squares)
            (loop s squares
                (set-square! g s (square-function s)))
         g))

    (define units (make-grid units-for-square))
    (define peers (make-grid peers-for-square))

    (define (grid->lists grid)
        (loop r digits
            (loop c digits
                (show-digits (get-square grid (cons r c))))))

    (define grid1 "003020600900305001001806400008102900700000008006708200002609500800203009005010300")
    (define grid2 "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......")

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
            (if (eq? (length values) num-squares)
                (zip squares values)
                (throw "Could not parse grid"))))

    (define amb (make-amb-function (curry throw "No solution")))
    (define fail amb)

    ; After removing d from s and its peers,
    ; does d now only appear in one place for the units
    ; of s? If so, "assign" to that place.
    (define (check-units grid s d)
        (fold-loop u (get-square units s)
                   g grid
            (let dplaces (filter-loop s u (has-digit? (get-square g s) d))
                 (match dplaces
                    () (fail) ; d cannot appear anywhere in this unit => contradiction
                    ; d only appears in 's' in this unit
                    ; So assign d to square s in values
                    (s) (assign! g s d )
                    ; No inference possible: do nothing.
                    _   g))))

    (define eliminate-peers!
        (lambda
            (grid s (d))
                (fold-loop peer (get-square peers s)
                           g grid
                            (eliminate! g peer d))
        ; More than one remaining - leave unchanged
            (grid . _) grid))

    (define (apply-rules grid s left)
        (if (none? left)
            (fail)
            (begin
                (set-square! grid s left)
                (eliminate-peers! grid s (show-digits left)))))

    (define (eliminate! grid s d)
        (define current (get-square grid s))
        ; This test required to terminate recursion from
        ; eliminate-peers!
        (if (has-digit? current d)
            (apply-rules grid s (remove-digit current d))
            grid))

    ; Return the 'values' that results from
    ; assigning d to square s
    (define (assign! grid s d)
        (define others (remove-digit (get-square grid s) d))
        (fold-loop d (show-digits others)
                   g grid
                   (eliminate! g s d)))

    (define (digit? d)
            (in d digits))

    (define (parse-grid grid-string)
        (fold-loop (s . d) (grid->values grid-string)
                   g (new-grid)
                (if (digit? d)
                    (assign! g s d)
                    g)))

    (define (solved? grid)
        (let/cc return
            (index-loop i num-squares 
                (if (solved-digit? (vector-ref grid i))
                    #t
                    (return #f)))
            #t))

    (define two-through-9 (cdr digits))

    (define (square-to-try grid)
        (let/cc return
            (loop num-missing two-through-9
                (loop s squares
                    (let possible (show-digits (get-square grid s))
                         (if (eq? (length possible) num-missing)
                             (return (cons s possible))
                             #f))))
            return "None missing"))

    (define write-line System.Console.WriteLine)
    (define (solve grid)
        (if (solved? grid)
            grid
            (with* ((s . digits) (square-to-try grid)
                    d (amb digits))
                   (write-line "Assiging {0} to {1}" d s)
                   (solve (assign! (copy-grid grid) s d)))))
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
            (set-square! g '(1 . 2) 23)
            (get-square g '(1 . 2))))

    (solved-true?
        #t
        (solved? (parse-grid grid1)))

    (solved-false?
        #f
        (solved? (parse-grid grid2)))

    ; We can solving using only technique 1 - 
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
        (grid->lists (parse-grid grid1)))

    ; Grid2 requires recursive search
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
        (grid->lists (solve (parse-grid grid2))))

)
