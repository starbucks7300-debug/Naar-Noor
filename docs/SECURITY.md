# Security Policy

## Supported Versions

We release patches for security vulnerabilities. Currently supported versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of The Lost Yeti Kitchen & Bar seriously. If you believe you have found a security vulnerability, please report it to us as described below.

### Where to Report

**Please do NOT report security vulnerabilities through public GitHub issues.**

Instead, please report them via email to: **security@thelostyeti.com**

### What to Include

Please include the following information in your report:

- Type of vulnerability
- Full paths of source file(s) related to the vulnerability
- Location of the affected source code (tag/branch/commit or direct URL)
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the vulnerability
- Suggested fix (if available)

### Response Timeline

- **Initial Response**: Within 48 hours
- **Status Update**: Within 7 days
- **Fix Timeline**: Depends on severity
  - Critical: 24-48 hours
  - High: 7 days
  - Medium: 30 days
  - Low: 90 days

## Security Measures

### Application Security

#### 1. Input Validation
- All user inputs are validated
- XSS protection implemented
- SQL injection prevention (when database is added)
- CSRF tokens for forms

#### 2. Authentication & Authorization
- Secure password hashing (when auth is added)
- JWT token implementation
- Role-based access control
- Session management

#### 3. Data Protection
- HTTPS enforcement
- Secure cookie flags
- Content Security Policy (CSP)
- X-Frame-Options header
- X-Content-Type-Options header

#### 4. API Security
- Rate limiting
- API key validation
- Request throttling
- Input sanitization

### Infrastructure Security

#### 1. Server Configuration
- Regular security updates
- Firewall configuration
- DDoS protection
- Intrusion detection

#### 2. Database Security
- Encrypted connections
- Principle of least privilege
- Regular backups
- Access logging

#### 3. CI/CD Security
- Secrets management
- Dependency scanning
- Container scanning
- Code signing

### Dependencies

#### Automated Scanning
- Dependabot enabled
- npm audit on every build
- OWASP dependency check
- Snyk integration

#### Update Policy
- Critical vulnerabilities: Immediate
- High vulnerabilities: Within 7 days
- Medium vulnerabilities: Within 30 days
- Low vulnerabilities: Next release

### Code Security

#### Static Analysis
- ESLint security rules
- TypeScript strict mode
- SonarQube scanning
- CodeQL analysis

#### Code Review
- All PRs require review
- Security checklist for reviewers
- Automated security checks
- Manual security review for sensitive changes

### Deployment Security

#### Production Environment
- Environment variable protection
- Secrets encryption
- Access control
- Audit logging

#### Build Process
- Signed commits
- Verified builds
- Immutable artifacts
- Supply chain security

## Security Best Practices

### For Contributors

1. **Never commit secrets**
   - Use environment variables
   - Add sensitive files to .gitignore
   - Use secret management tools

2. **Validate all inputs**
   - Client-side validation
   - Server-side validation
   - Sanitize user data

3. **Follow secure coding guidelines**
   - OWASP Top 10
   - Angular security best practices
   - TypeScript security patterns

4. **Keep dependencies updated**
   - Regular npm updates
   - Review security advisories
   - Test after updates

### For Users

1. **Keep your browser updated**
2. **Use strong passwords** (when auth is added)
3. **Enable two-factor authentication** (when available)
4. **Report suspicious activity**

## Security Headers

The application implements the following security headers:

```
Content-Security-Policy: default-src 'self'; script-src 'self' 'unsafe-inline' cdn.tailwindcss.com code.iconify.design; style-src 'self' 'unsafe-inline' fonts.googleapis.com; font-src fonts.gstatic.com; img-src 'self' data:; frame-src my.spline.design;
X-Frame-Options: SAMEORIGIN
X-Content-Type-Options: nosniff
X-XSS-Protection: 1; mode=block
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: geolocation=(), microphone=(), camera=()
```

## Vulnerability Disclosure Policy

### Coordinated Disclosure

We follow a coordinated disclosure process:

1. **Report received** - Acknowledgment sent
2. **Validation** - Vulnerability confirmed
3. **Fix development** - Patch created
4. **Testing** - Fix verified
5. **Release** - Security update deployed
6. **Disclosure** - Public announcement (after fix)

### Public Disclosure

After a fix is released:
- Security advisory published
- CVE assigned (if applicable)
- Credit given to reporter (if desired)
- Details added to CHANGELOG.md

## Security Contacts

- **Security Team**: security@thelostyeti.com
- **Emergency Contact**: +44 (0) 1481 123456
- **PGP Key**: Available on request

## Compliance

### Standards
- OWASP Top 10
- GDPR (when handling EU data)
- PCI DSS (when processing payments)
- WCAG 2.1 Level AA

### Audits
- Annual security audit
- Penetration testing
- Code review
- Dependency audit

## Security Checklist

### Before Deployment

- [ ] All dependencies updated
- [ ] Security scan passed
- [ ] No secrets in code
- [ ] HTTPS configured
- [ ] Security headers set
- [ ] Error handling implemented
- [ ] Logging configured
- [ ] Backup system tested
- [ ] Access controls verified
- [ ] Documentation updated

### Regular Maintenance

- [ ] Weekly dependency updates
- [ ] Monthly security review
- [ ] Quarterly penetration test
- [ ] Annual security audit

## Incident Response

### In Case of Security Breach

1. **Immediate Actions**
   - Isolate affected systems
   - Preserve evidence
   - Notify security team
   - Begin investigation

2. **Communication**
   - Internal notification
   - User notification (if data affected)
   - Regulatory notification (if required)
   - Public disclosure (when appropriate)

3. **Recovery**
   - Apply fixes
   - Restore services
   - Monitor for recurrence
   - Post-incident review

## Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Angular Security Guide](https://angular.io/guide/security)
- [npm Security Best Practices](https://docs.npmjs.com/security-best-practices)
- [TypeScript Security](https://www.typescriptlang.org/docs/handbook/security.html)

## Acknowledgments

We thank the security researchers who help keep our project safe:

- [List will be maintained as reports come in]

---

**Last Updated**: 2026-03-26

For questions about this policy, contact: security@thelostyeti.com
