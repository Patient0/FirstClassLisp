(define nil '())
(define list (lambda x x))
(define car (lambda ((a . b)) a))
(define cdr (lambda ((c . d)) d))
(define nil? (lambda (()) #t _ #f))
(define pair? (lambda ((_ . _)) #t _ #f))


; Now, let's implement simple non-nested quasiquote in terms of Lisp itself
; We need it quite early because writing macros without quasiquote
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

; Our let macro is like the one in arc - just
; a single variable, single expression, and no
; nesting.
; We can define "with" later as a macro that
; expands into individual sublets.
(define let (macro
  (lambda (var value body)
    `((,lambda (,var) ,body) ,value))))

; Now add support for multiple sub-statements in define:
; Whenever we see (define x expr1 expr2 ...)
; we'll expand it to
; (raw-define x (begin expr1 expr2 ...))
(define define
    (macro
        ; Because define itself mutates the environment,
        ; we have to capture the original 'define' here
        ; before we 'hide' it behind our macro replacement.
        ; Otherwise, we go into an infinite loop when expanding.
        (let raw-define define
            (lambda (symbol . exprs)
                `(,raw-define ,symbol (,begin ,@exprs))))))

; Y combinator allows us to write recursive code without
; mutating the environment.
(define Y 
    (lambda (m)
       (let z (lambda (f) (m (lambda (a) ((f f) a))))
	     (z z))))
; We could use the Y combinator here, but because we are defining
; 'length' using a define form, we can just recurse directly
(define length
    (define length-tail
        ; Here, we make use of the "pattern matching" in lambda
        (lambda (so-far ()) so-far
                (so-far (x . y))
                    (length-tail (+ 1 so-far) y)))
    (lambda (list)
        (length-tail 0 list)))

(define fold-right
    (lambda (op initial ()) initial
            (op initial (x . y)) (fold-right op (op x initial) y)))

(define reverse
    (define reverse-tail
        (lambda (so-far ()) so-far
                (so-far (x . y))
                    (reverse-tail (cons x so-far) y)))
    (lambda (l)
        (reverse-tail '() l)))

(define mapcar
    (lambda (f list)
        ; combiner creates a binary function which
        ; takes the element, the list, and returns
        ; the function applied to the element cons'ed
        ; onto the existing list
        (let combiner
            (lambda (x list)
                (cons (f x) list))
            (reverse (fold-right combiner () list)))))

(define map
    (define map-tail
        (lambda
            (f so-far (() . rest))
                so-far
            (f so-far ll)
                (map-tail f (cons (apply f (mapcar car ll)) so-far)
                            (mapcar cdr ll))))
    (lambda (f . ll)
        (reverse (map-tail f '() ll))))
