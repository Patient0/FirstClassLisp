(define (let-bindings bindings)
    (define splice
        (lambda (bindings      ())
                    bindings
                ((vars values) (var value . rest))
                    (splice (list (cons var vars) (cons value values)) rest)))
    (let (vars values) (splice '(() ()) bindings)
        (list vars (cons list values))))

(define-macro with2 (bindings . body)
    `(let ,@(let-bindings bindings) ,@body))
