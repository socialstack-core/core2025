# UI/GeneralStyle

Place project-specific styling here. Use of multiple `.scss` files is strongly encouraged for separation of concerns.

## Notes

- note that all `.scss` files across the project will be compiled and combined into a single (compressed, on production) stylesheet
- if the order of styles becomes important, note that each individual `.scss` file can be updated with a value of 0-100 to indicate priority
- for instance, rules within `foo.10.scss` will appear before rules defined within `bar.20.scss`
- if not specified, the default priority for an `.scss` file is 100
- UI/GlobalStyle provides predefined SASS functions, methods and media query helpers which you are strongly advised to make use of :)
- note that the core styles and the more recently-updated common components make heavy use of CSS properties (aka CSS variables)
- more often than not, elements of the UI can be tweaked by making changes to these CSS properties, rather than attempting to rewrite rules

## Important!

Core styling is defined within UI/GlobalStyle - note that this is a shared component, and as such, any changes are expected to be shared across ALL SocialStack projects.
The core styling has been structured in such a way that in the majority of cases, direct edits to UI/GlobalStyle should not be necessary:

- core styles are defined within CSS @layers; this should make overriding these styles easier, with less dependency on excessively-specific rules or the dreaded !important flag
- styles written _outside_ of CSS layers (e.g. those for the project) should automatically gain more specificity than those within layers

## TL;DR

Keep project-specific styles under UI/GeneralStyle and if you need to change anything within UI/GlobalStyle (or, indeed, styling for _any_ shared component), 
please inform the wider dev team so that breaking changes / knock-on effects in other projects can be kept to a minimum. Thanks!