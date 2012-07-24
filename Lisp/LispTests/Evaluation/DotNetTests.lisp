(setup
    ; Bring in all static methods in the mscorlib assembly
    (define (invoke-instance method . args)
        (apply (make-instance-method method) args)))
(tests
    ; Call the primitive invoke-method function
    (invoke-instance-simple
        "42"
        (invoke-instance "ToString" 42))

    (invoke-instance-with-args1
        #t
        (invoke-instance "Equals" "hello world" "hello world"))
    (invoke-instance-with-args2
        #f
        (invoke-instance "Equals" "hello" "nothello"))

    (invoke-instance-macro
        "42"
        (.ToString 42))
    ; Call a static method returning a value type (Boolean)
    (static-method
        #t
        (System.Convert/ToBoolean "true"))
    ; Call a static method returning void
    (static-method-void
        ()
        (System.Console/WriteLine "hello"))
    ; Call an instance method (Equals) on a reference type (String)
    (instance-method-reftype
        #t
        (.Equals "hello" "hello"))
    ; Call an instance method (Equals) on a value type (Int32), passing a value type (Int32)
    (instance-method-valuetype
        #t
        (.Equals 1 1))
    ; Call an instance method (Equals) on a value type (Int32), passing a reference type (String)
    (instance-method-valuetype-boxed-arg
        #f
        (.Equals 1 "hello"))

    ; Evaluation always has to be via the main stack rather than
    ; creating nested "Evaluator" instances.
    ; Otherwise, as evidenced below, the "exception" (one example
    ; of how we can use continuations), is only "thrown" to the
    ; .Net method call site rather than the original source
    ; of the continuation.
    (args-obey-continuations
        "failed"
        (let-cc error
            (.Equals "failed" (error "failed"))))

    (static-method-continuations
        "failed"
        (let-cc error
            (System.Console/WriteLine (error "failed"))))

    (static-method-is-function
        (#t #f)
        (map System.Convert/ToBoolean '("true" "false")))

    (instance-method-as-function
        ("23" "34")
        (map (make-instance-method "ToString") '(23 34)))

    ; This expands into the above
    (instance-method-macro-as-function
        ("23" "34")
        (map .ToString '(23 34)))

    (multi-argument-higher-order
        (#t #f #t)
        (map .Equals '(1 "two" 3) '(1 2 3)))

    (invoke-constructor
        "This is an error"
        (.get_Message (new System.Exception "This is an error")))

    (params
        "4 * 5 is 20"
        (System.String/Format "{0} * {1} is {2}" 4 5 (* 4 5)))

    ; We have a quandary: What to do if the input parameter
    ; is *not* an atom and it's passed as a parameter to a reflection
    ; function? Similarly, if a "reflection" function returns
    ; a Datum, should it be wrapped as an "atom" or not?

    ; For now, let's go with "smart" behaviour.
    ; If this doesn't do what's desired, we will also expose
    ; a "atom" function explicitly for wrapping a Datum so that
    ; it looks like an atom.
    (datum-types-static "(1 2 3)" (System.String/Format "{0}" '(1 2 3)))
    (datum-types-instance "(1 2 3)" (.ToString '(1 2 3)))

    ; If we want to treat an atom explicitly as an atom (no implicit unwrapping),
    ; wrap it using the 'atom' function (i.e. to turn off the
    ; builtin "unwrapping" of atoms that is the default).
    (atom-wrap
        "LispEngine.Datums.Atom"
        (.ToString (.GetType (atom 1))))

    (get-type
        "System.Console"
        (.get_FullName (get-type "System" "Console")))

    (get-type-macro
        "System.Console"
        (.get_FullName System.Console))

    ; A macro reference to a non-existent type should throw
    ; an error
    (non-existent-type
        "ERROR"
        (try
            System.XXXX
        catch ex
            "ERROR"))

    ; Don't just look in calling assembly - look in all
    ; loaded assemblies in the current app domain.
    (uses-assembly-get-type
        "System.Diagnostics.Stopwatch"
        (.get_FullName System.Diagnostics.Stopwatch))
)
