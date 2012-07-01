(ref mscorlib)
(define console-i/o-port (open-input-stream (System.Console.get_In)))
(define (display result)
    (System.Console.WriteLine "-> {0}" result))
(define (prompt)
    (System.Console.Write "FCLisp> "))

(let/cc return
    (begin
        (define (exit)
            (return nil))
        (define (repl)
            (prompt)
            (with (expr (read console-i/o-port)
                   result (eval expr (env)))
                   (display result))
            (repl))
        (repl)))
