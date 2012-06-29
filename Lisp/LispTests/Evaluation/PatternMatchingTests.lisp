; Our lambda f-expression does full "pattern" matching
; on all of its arguments. If you already have to implement
; the standard cases (lambda x lambda (x . y) lambda (x y . z))
; then implementing full recursive pattern matching is not
; really any more difficult, and makes for substantially
; simpler code everywhere else.
(tests
    (patternMatch1 6
            ((lambda (a (b c)) c) 4 (list 5 6)))
    (patternMatch2 6
            ((lambda ((a b) c) b) (list 5 6) 4))
    (patternMatch3 5
            ((lambda (((a))) a) (list (list 5))))
    (caseLambdaPair #t
            ((lambda ((x . y)) #t x #f) (list 3 4)))
    (caseLambdaPairWithAtom #f
            ((lambda ((x . y)) #t x #f) 3))
    (carUsingPatternMatch 3
            ((lambda ((x . y)) x) '(3 . 4)))
    (cdrUsingPatternMatch 4
            ((lambda ((x . y)) y) '(3 . 4)))
    (lambdaAtomArgs 2
            ((lambda (1) 2) 1))
    (pairBindingChecksForNull #t
            ((lambda ((#f . x)) life ((y . x)) y) '(#t 3)))
    ; We extend the 'quote syntax for lambda binding
    ; to allow us to pattern match against symbols as well
    ; as atoms
    (lambdaSymbolBind 26
            ((lambda ('some-symbol x) x)
                'some-symbol 26))
    (lambdaSymbolCase 3
            ((lambda ('add1 x) (+ 1 x)
                     ('subtract1 x) (- x 1))
                    'subtract1 4)))
