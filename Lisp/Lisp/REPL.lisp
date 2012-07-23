(define prev-stats (!get-statistics))
(define (log-steps)
    (with (stats (!get-statistics)
           delta (!get-statistics-delta prev-stats))
        (set! prev-stats stats)
        (write-line "{0}" delta)))
           
(define (display-error msg c)
    (define (indent-list continuation-fn)
        (loop f (continuation-fn c)
            (writeerr "\t{0}" f)))
    (writeerr "ERROR: {0}" msg)
    (writeerr "Tasks:")
    (indent-list task-descriptions)
    (writeerr "Results:")
    (indent-list pending-results)
    nil)

; Create a new environment with the specified definitions
(define (extend e . definitions)
    (define (extend1 (symbol definition) e)
        (eval
            `(,begin
                (,define ,symbol ,definition)
                (,env))
            e))
    (fold-right extend1 e definitions))

(define (make-debug-env (msg c))
    (extend (get-env c)
            `(trace ,(curry display-error msg c))
            `(resume ,c)))

(define last-error nil)
; Rudimentary repl that lets you evaluate
; expressions within the context of "last-error".
(define (debug)
    (if (nil? last-error)
        "Nothing to debug"
        (repl "debug> " (make-debug-env last-error))))

(define (repl prompt repl-env)
    (define prompt (curry write prompt))
    (let-cc return
        (define env-with-exit-and-debug
            (extend repl-env
                `(exit ,(curry return nil))
                `(debug ,debug)))
        (define (check-read)
            (let next (read console)
                (if (eof-object? next)
                    (return nil)
                next)))
        (define (repl-eval expr)
                (eval expr env-with-exit-and-debug))

        (define (loop)
            (try
                (prompt)
                (with* (expr (check-read)
                        result (repl-eval expr))
                  (display result)
                  (log-steps))
             catch error
                (set! last-error error)
                (writeerr "ERROR: {0}" (car error))
                (writeerr "(debug) to enter debug repl"))
            (loop))
        (loop)))

(repl "FCLisp> " global-env)
