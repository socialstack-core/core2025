# UI/GlobaStyle

Defines core styling for all SocialStack projects. Note that for the vast majority of the time, styling changes should be limited to UI/GeneralStyle,
as changes here can (and will!) affect other SocialStack projects. If you think you've spotted a bug / shortcoming with the core styling defined herein,
please inform the wider development team! Thanks :)

## Notes

- core styles are defined within CSS @layers; this should make overriding these styles easier, with less dependency on excessively-specific rules or the dreaded !important flag
- styles written _outside_ of CSS layers (e.g. those for the project defined under UI/GeneralStyle) should automatically gain more specificity than those within layers
- provides predefined SASS functions, methods and media query helpers which you are strongly advised to make use of :)

## Important!

Keep project-specific styles under UI/GeneralStyle and if you need to change anything within UI/GlobalStyle (or, indeed, styling for _any_ shared component), 
please inform the wider dev team so that breaking changes / knock-on effects in other projects can be kept to a minimum. Thanks!