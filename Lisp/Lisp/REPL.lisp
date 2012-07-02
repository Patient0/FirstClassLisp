(ref mscorlib)
(define console (open-input-stream (System.Console.get_In)))
(define (display result)
    (System.Console.WriteLine "-> {0}" result))
(define (display-error msg)
    (.WriteLine (System.Console.get_Error) "ERROR: {0}" msg))
(define (prompt)
    (System.Console.Write "FCLisp> "))

(let/cc return
    (begin
        (define (exit)
            (return nil))
        (define repl-env (env))
        ; Read an expression, but exit the loop
        ; if it's eof.
        (define (check-read)
            (let next (read console)
                 (if (eof-object? next)
                     (exit)
                     next)))

        ; If an error occurs, write to stderr and skip
        ; the display step.
        (define (eval-and-display expr)
            (let/cc abort
                (begin
                    (define (error-handler msg)
                        (display-error msg)
                        (abort nil))
                    (display (eval expr repl-env error-handler)))))

        (define (repl)
            (prompt)
            (with (expr (check-read))
                  (eval-and-display expr))
            (repl))
        (repl)))
