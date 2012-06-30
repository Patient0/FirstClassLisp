; We do this to work around the reader - need to get
; '.' as a symbol
(define-macro define-dot (definition)
    `(,define ,(string->symbol ".") ,definition))
(define-dot (macro
    (lambda
        ; Overload the syntax of this macro:
        ; Allow implicit "currying" if no instance has been specified
        ; This allows nicer syntax such as:
        ; (map (.ToString) '(1 2 3))
        (method)
            `(,curry ,invoke-instance ,(symbol->string method))
        (method instance . args)
            `(,invoke-instance ,(symbol->string method) ,instance ,@args))))
