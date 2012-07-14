(let/cc error
    (define amb-fail (curry error "exhausted"))
    (define (assert condition)
        (if (not condition)
            (amb-fail)
            #t))

    ; Based on
    ; http://c2.com/cgi/wiki/?AmbSpecialForm
    (define amb
        (define expand-amb (lambda
            ()  (list force amb-fail)
            (x) x
            (x y)
                (with (old-fail (gensym)
                       c (gensym))
                `(,let ,old-fail amb-fail
                    (,force
                        (,let/cc ,c
                            (,set! amb-fail
                                (,make-thunk
                                    (,set! amb-fail ,old-fail)
                                    (,c (,make-thunk ,y))))
                            (make-thunk ,x)))))
            (x . rest)
                `(,amb ,x (,amb ,@rest))))
     (macro expand-amb))

    ; Search for a pythagorean triple
    (let solution
        (with (square (lambda (x) (* x x))
               a (amb 1 2 3 4 5 6 7)
               b (amb 1 2 3 4 5 6 7)
               c (amb 1 2 3 4 5 6 7))
                (write-line "Trying a: {0} b: {1} c: {2}" a b c)
                (assert (eq? (square c) (+ (square b) (square a))))
            (list a b c))
        (write-line "Solution: {0}" solution))

    (repl "amb> " (env))
)
