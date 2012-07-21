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

    (eof-detect
        (#t #f)
        (map (compose eof-object? read open-input-string)
                 '("" "(* 5 3)")))

    ; The REPL would switch to "EOF" as soon as an unrecognized
    ; token was read, which is not very useful behaviour in
    ; a REPL.
    (unrecognized-token
        5
        (let s (open-input-string "@5")
            (try
                (read s)
             catch msg
                (read s))))

)
