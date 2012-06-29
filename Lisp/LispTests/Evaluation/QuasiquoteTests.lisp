; Quasiquote test cases
(setup
    (define life 42))
(tests
    (quasiquoteAtom 1 `1)
    (quasiquoteSymbol x `x)
    (quasiquoteList (1 2) `(1 2))
    (quasiquoteUnquote 42 `,life)
    (quasiquoteUnquoteUsingLet 6
        (let x 6 `,x))
    (quasiquoteUnquoteList (1 6)
        (let x 6
            `(1 ,x)))
    (quasiquoteQuoted (list a 'a)
        (let name 'a
            `(list ,name ',name)))
    (quasiquoteSplicingSimplest
        (1 2)
            `(,@(list 1 2)))
    (quasiquoteSplicing
        (1 2 3 4 5)
            `(1 ,@(list 2 3) 4 5))
    (mapQuasiQuoteTest (1 2 3 1 4 9 4 5 6)
        `(1 2 3 ,@(map (lambda (x) (* x x)) '(1 2 3)) 4 5 6)))
