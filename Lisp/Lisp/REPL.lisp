(ref mscorlib)
(define console (open-input-stream (System.Console.get_In)))
(define (display result)
    (System.Console.WriteLine "-> {0}" result))

(define (display-error msg c)
    (define stderr (System.Console.get_Error))
    (define writeerr (curry .WriteLine stderr))
    (define (indent-list continuation-fn)
        (loop f (continuation-fn c)
            (writeerr "\t{0}" f)))
    (writeerr "ERROR: {0}" msg)
    (writeerr "Tasks:")
    (indent-list task-descriptions)
    (writeerr "Results:")
    (indent-list pending-results)
    nil)

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

        (define (repl)
            (try
                (prompt)
                (with (expr (check-read)
                       result (eval expr repl-env))
                      (display result))
             catch (msg c)
                (display-error msg c))
            (repl))
        (repl)))
