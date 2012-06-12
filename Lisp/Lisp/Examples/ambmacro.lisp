; Original example using plain 'amb
(let/cc error
    (begin
        (define amb-fail (lambda () (error 'exhausted)))
        (define (assert condition)
            (if (not condition)
                (amb-fail)
                #t))
        (define (delay x)
            (lambda () x))
        (define (force f)
            (f))
        ; Based on
        ; http://c2.com/cgi/wiki/?AmbSpecialForm
        ; Haven't got it quite working yet
        (define amb (macro
            (lambda ()  `(,amb-fail)
                    (x) x
                    (x . y)
                        (let old-fail amb-fail
                            `(,force
                            (,let/cc cc
                                    (,begin
                                    (,set! amb-fail
                                        (,delay
                                            (,begin
                                            (,set! amb-fail ,old-fail)
                                            (,begin
                                                (,set! amb-fail ,old-fail)
                                                (cc (,delay ,y))))))
                                    (,delay ,x))))))))

        (with (a (amb 2)
               b (amb 2))
            (begin
                (display `(a ,a))
                (display `(b ,b))
                ;(assert (eq? b 2))
                ; This blows up - '3' is not callable
                ; missing a delay... 
                ; It's too complicated - need to reduce it down to
                ; some simpler functions/macros to get it working.
                ;(assert (eq? b 3))
))))
