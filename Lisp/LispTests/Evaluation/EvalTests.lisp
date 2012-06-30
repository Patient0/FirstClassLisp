(tests
    ; "(env)" is an f-expression that returns the current environment.
    ; This environment is currently "opaque".
    ; "eval" is then a regular function that takes this environment
    ; and evaluates the expression.
    (simple-eval
        15
        (eval '(* 3 5) (env)))
    (simple-read
        (* 5 3)
        (read (open-input-string "(* 5 3)")))
)
