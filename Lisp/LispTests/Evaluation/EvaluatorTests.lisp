; This file contains test cases used in EvaluatorTest. It is expected to
; contain a list of lists.
; Each list is a test case.
; The first element is the name
; The second is expected result.
; The third is the expression to evaluate.
(atom 5 5)
(boolAtomTrue #t #t)
(boolAtomFalse #f #f)
; Many of these tests require an arbitrary symbol
; to be defined. So we'll use 'life' as our symbol,
; which is mapped to the integer '42' in the environment.
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
(eqTrue? #t
		(eq? 4 4))
(eqFalse? #f
		(eq? 4 3))
(eqSymbolTest #t
    (eq? 'somesymbol 'somesymbol))
(eqSymbolTestFalse #f
    (eq? 'someSymbol 'someOtherSymbol))
(eqQuotedSymbolWithUnquoted #t
    (let x 'unquote
        (eq? x 'unquote)))
; But we don't want eq? to be traversing
; arbitrarily large structures for us.
(eqIsFlat #f
		(eq? (list 1 2) (list 1 2)))
(ifTrue? 5
		(if #t 5 undefined))
(ifFalse? 5
		(if #f undefined 5))
; Discovered quite late on that the if
; F-expression wasn't actually evaluate either the
; true or the false case!
(ifExpressionTrue 42 
        (if (eq? life 42) life 23))
(ifExpressionFalse 42 
        (if (eq? life 98) 5 life))
; We'll make our 'if' like a 'cond' - allow multiple
; clauses until one matches. Should always be an
; odd number of clauses.
(ifIsLikeCond 42
        (if (eq? life 23) not-this-one
            (eq? life 44) not-this-one
            (eq? life 48) no-not-this-either
            42))
(quotedList (3 4) '(3 4))
(append (1 2 3 4)
    (append '(1 2) '(3 4)))
(appendMultiple (1 2 3 4 5 6)
    (append '(1 2) '(3 4) '(5 6)))
; It's very convenient if append works even for a single list
(appendSingle (1 2)
    (append '(1 2)))
(appendNone ()
    (append))
(quotedAtom 3 '3)
(quotedQuote '3 ''3)
(quotedSymbol x 'x)
(let 3 (let x 3 x))
(letEvaluatesBody #t
		(let x 3 (eq? x 3)))
; Our nil *only* matches nil. Anything else is "not nil")
(nil1 #t (nil? '()))
(nil2 #f (nil? 5))
(nil3 #f (nil? #f))
(pair1 #t (pair? '(1 . 2)))
(pair2 #f (pair? 5))
