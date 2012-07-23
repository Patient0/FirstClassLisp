(define nil '())
(define list (lambda x x))
(define car (lambda ((a . b)) a))
(define cdr (lambda ((c . d)) d))
(define nil? (lambda (()) #t _ #f))
(define pair? (lambda ((_ . _)) #t _ #f))
(define display log)

; Now, let's implement simple non-nested quasiquote in terms of Lisp itself
; We need it quite early because writing macros with*out quasiquote
; is extremely painful!
; Using the builtin pattern matching of our lambda primitive makes
; this significantly simpler to implement.
(define expand-quasiquote
    (lambda
        (('unquote e))
            e
        ((('unquote-splicing x) . y))
            (list append x (expand-quasiquote y))
        ((x . y))
            (list cons (expand-quasiquote x) (expand-quasiquote y))
        x
            (cons quote x)))
(define quasiquote
    (macro expand-quasiquote))

; We use define followed by macro and a lambda an awful lot - so 
; let's define a macro to ease the overhead in
; writing macros!
(define define-macro
    (macro (lambda (name args . exprs)
            `(,define ,name
                (,macro (,lambda ,args (,begin ,@exprs)))))))


; Our let macro is like the one in arc - just
; a single variable, single expression, and no
; nesting.
; We can define "with*" later as a macro that
; expands into individual sublets.
(define-macro let (var value . body)
    `((,lambda (,var) (,begin ,@body)) ,value))

; Our pattern matching is powerful enough to define 'if'
; as a macro. In fact, it can even handle
; "multicase" if, by expanding the remaining
; clauses into itself.
(define if (macro
    (lambda (condition true-case false-case)
            ; Base case. Anything not false is considered 'true'
                `((,lambda (#f) ,false-case
                            _   ,true-case) ,condition)
            ; Multiple clauses. Expand into recursive if statements.
            (condition true-case . remainder)
                `(,if ,condition ,true-case
                       (,if ,@remainder)))))

; Now add support for multiple sub-statements in define:
; Whenever we see
; (define x expr1 expr2 ...)
; we'll expand it to
; (raw-define x (begin expr1 expr2 ...))
; Also, whenever we see
; (define (name arg1 arg2) expr1 expr2)
; we'll expand it to 
; (define name (lambda arg1 arg2) (begin expr1 expr2))
; This is the traditional syntax used in SICP et al.
; However, we can't support multiple bodies in our
; lambdas with*out screwing up our nice 'case lambda'
; syntax which I find more useful than being able to
; define multiple bodies in a lambda. You can aways
; use 'begin' explicitly if need be.
(define define
    (macro
        ; Because define itself mutates the environment,
        ; we have to capture the original 'define' here
        ; before we 'hide' it behind our macro replacement.
        ; Otherwise, we go into an infinite loop when expanding.
        (let raw-define define
            (lambda
                ; Traditional function definition
                ((fn . args) . exprs)
                    `(,raw-define ,fn (,lambda ,args (,begin ,@exprs)))
                (symbol . exprs)
                    `(,raw-define ,symbol (,begin ,@exprs))))))

(define (let-bindings bindings)
    (define splice
        (lambda (bindings      ())
                    bindings
                ((vars values) (var value . rest))
                    (splice (list (cons var vars) (cons value values)) rest)))
    (let (vars values) (splice '(() ()) bindings)
        (list vars (cons list values))))

; 'with' macro allows multiple bindings - but the bindings
; cannot see each other. i.e.
; (with (x 5 y x) ... ) won't work.
; However, this version is probably more efficient than with* below.
(define-macro with (bindings . body)
    `(let ,@(let-bindings bindings) ,@body))

; 'with*' macro decomposes recursively into nested 'let' statements
; So 'later' definitions can see earlier definitions.
; i.e.
; (with* (x 5 y x) ... ) will work.
(define with* (macro
    (lambda (() body)
                body
            ((var . (expr . bindings)) . body)
                `(,let ,var ,expr (,with* ,bindings (,begin ,@body))))))

; We cannot use "/" as this is overloaded to allow static method
; access ala Clojure style.
(define call-cc call-with-current-continuation)

; let-cc provides "escape" functionality
(define-macro let-cc (var . body)
    `(,call-cc (,lambda (,var) (,begin ,@body))))

; Function that can be used to expand a (possibly)
; macro-ized expression. Useful for debugging
; macros. The 'trick' part is the "unmacro"
; builtin which is the inverse function of 'macro'.
(define expand
    (lambda (env (fexpr . args))
                (with* (macro-expr (eval fexpr env)
                       macro-fn (unmacro macro-expr))
                      (if (nil? macro-fn)
                          (cons fexpr args)
                          (apply macro-fn args)))
            (env other) other))

; In the common case of only wanting to
; dispatch on the pattern of one variable,
; define a convenient macro to de-nest
; the arguments that would otherwise
; be required in a plain lambda expression
(define-macro match (var . cases)
    (define de-nest
        (lambda
            (()) ()
            ((pattern . (body . remaining)))
                `(,(list pattern) ,body ,@(de-nest remaining))))
    `((,lambda ,@(de-nest cases)) ,var))

(define (not expr)
    (match expr
        #f #t
        _ #f))

