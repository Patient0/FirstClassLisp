; Our lambda macro actually does full "pattern" matching
; on all of its arguments - this was actually 
; almost as simple to
; implement as the standard, which is a subset.
; If then allowed multiple alternative argument lists
; we could then support pattern matching.
(patternMatch1 6
		((lambda (a (b c)) c) 4 (list 5 6)))
(patternMatch2 6
		((lambda ((a b) c) b) (list 5 6) 4))
(patternMatch3 5
		((lambda (((a))) a) (list (list 5))))
; It was easy to implement pattern matching as a builtin for lambda from the start)
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
                'subtract1 4))
