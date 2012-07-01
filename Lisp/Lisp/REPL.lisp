(ref mscorlib)
(define console (open-input-stream (System.Console.get_In)))
(define (display result)
    (System.Console.WriteLine "-> {0}" result))
(define (prompt)
    (System.Console.Write "FCLisp> "))

(let/cc return
    (begin
        (define (exit)
            (return nil))
        ; Read an expression, but exit the loop
        ; if it's eof.
        (define (check-read)
            (let next (read console)
                 (if (eof-object? next)
                     (exit)
                     next)))
        (define (repl)
            (prompt)
            (with (expr (check-read)
                   result (eval expr (env)))
                   (display result))
            (repl))
        (repl)))
