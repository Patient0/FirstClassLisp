(define global-env (env))
(define repl-run
    (let standard-run run
        (lambda
            (filename run-environment)
                (standard-run filename run-environment)
            (filename)
                (standard-run filename global-env))))
; Redefine 'run' to a user-friendly version
; that defaults to the repl-environment for
; evaluation
(define run repl-run)

(define prev-stats (!get-statistics))
(define (log-steps elapsed)
    (with (stats (!get-statistics)
           delta (!get-statistics-delta prev-stats))
        (set! prev-stats stats)
        (write-line "{0} Elapsed: {1}" delta elapsed)))
           
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

(define Stopwatch System.Diagnostics.Stopwatch)

(define clear System.Console/Clear)

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
                        stop-watch (Stopwatch/StartNew)
                        result (repl-eval expr))
                  (display result)
                  (log-steps (.get_Elapsed stop-watch)))
             catch error
                (set! last-error error)
                (writeerr "ERROR: {0}" (car error))
                (writeerr "(debug) to enter debug repl"))
            (loop))
        (loop)))

(repl "FCLisp> " global-env)
