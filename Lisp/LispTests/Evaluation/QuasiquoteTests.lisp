; Quasiquote test cases
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
