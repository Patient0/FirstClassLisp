; This file contains test cases used in EvaluatorTest. It is expected to
; contain a list of lists.
; Each list is a test case.
; The first element is the name
; The second is expected result.
; The third is the expression to evaluate.
(atom 5 5)
(boolAtomTrue #t #t)
(boolAtomFalse #f #f)
(symbol 42 life)
(identityFunction 5 ((lambda (x) x) 5))
(constantFunction 6 ((lambda () 6)))
(lambdaList (1 2 3) ((lambda x x) 1 2 3))
(list (1 2 3) (list 1 2 3))
(cons (3 . 4) (cons 3 4))
(recursiveFunctions (3 4 . 5) (cons 3 (cons 4 5)))
(apply (3 . 4) (apply cons (list 3 4)))
(dotArgList 4 ((lambda (x . y) x) 4))
(carList 4 (car (list 4 5)))
(cdrList (5) (cdr (list 4 5)))
; Our lambda macro actually does full "structure" matching)
; on all of its arguments - this was actually as simple to)
; implement as the standard, which is a subset.)
; If then allowed multiple alternative argument lists)
; we could then support pattern matching.)
(structuredMatch1 6
		((lambda (a (b c)) c) 4 (list 5 6)))
(structuredMatch2 6
		((lambda ((a b) c) b) (list 5 6) 4))
(structuredMatch3 5
		((lambda (((a))) a) (list (list 5))))
(eqTrue? #t
		(eq? 4 4))
(eqFalse? #f
		(eq? 4 3))
(eqIsFlat #f
		(eq? (list 1 2) (list 1 2)))
(ifTrue? 5
		(if #t 5 undefined))
(ifFalse? 5
		(if #f undefined 5))
(quotedList (3 4) '(3 4))
(quotedAtom 3 '3)
(quotedQuote '3 ''3)
(let 3 (let x 3 x))
(letEvaluatesBody #t
		(let x 3 (eq? x 3)))
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
; Our nil *only* matches nil. Anything else is "not nil")
(nil1 #t (nil? '()))
(nil2 #f (nil? 5))
(nil3 #f (nil? #f))
(pair1 #t (pair? '(1 . 2)))
(pair2 #f (pair? 5))
