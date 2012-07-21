(setup
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

    (define (get-square grid s)
            (vector-ref grid (index s)))

    (define (set-square! grid s ds)
            (vector-set! grid (index s) ds))

    (define copy-grid vector-copy)
    (define (new-grid)
        (copy-grid empty-grid))
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
)
