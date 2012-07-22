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

    (define (digit-set . digits)
        (fold-loop d digits
                   ds 0
                   (add-digit ds d)))

    (define all-digits (apply digit-set digits))
    (define none (digit-set))

    (define zero? (curry eq? 0))
    (define not-zero? (compose2 not zero?))

    (define (remove-digit ds d)
        (bit-and ds (- all-digits (digit-bit d))))

    (define (has-digit ds d)
        (not-zero? (bit-and (digit-bit d) ds)))

    ; Inverse function of digit-set constructor
    (define (show-digits ds)
        (filter-loop d digits
            (has-digit ds d)))

    ; Grid representation. Use a vector
    ; and row/column arithmetic.
    (define empty-grid
        (make-vector 81 all-digits))

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
        (let g (make-vector 81)
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

    (define eliminate-peers!
        (lambda
            (grid s (d))
                (fold-loop peer (get-square peers s)
                           g grid
                            (eliminate! g peer d))
        ; More than one remaining - leave unchanged
            (grid . _) grid))

    (define (eliminate! grid s d)
        (define current (get-square grid s))
        ; This test required to terminate recursion from
        ; eliminate-peers!
        (if (has-digit current d)
            (begin
                (define left (remove-digit current d))
                (set-square! grid s left)
                (eliminate-peers! grid s (show-digits left)))
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

    (define (parse-grid grid)
        (fold-loop (s . d) (grid->values grid)
                   g (new-grid)
                (if (digit? d)
                    (assign! g s d)
                    g)))
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

    (grid-get-set
        23
        (begin
            (define g (new-grid))
            (set-square! g '(1 . 2) 23)
            (get-square g '(1 . 2))))

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

)
