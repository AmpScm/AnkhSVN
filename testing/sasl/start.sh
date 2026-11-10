#!/bin/sh
set -eu
mkdir -p /repos /etc/sasl2
svnadmin create /repos/control
svnadmin create /repos/sasl
cat > /repos/control/conf/svnserve.conf <<'EOF'
[general]
anon-access = none
auth-access = write
password-db = passwd
realm = ankh-sasl-test
EOF
cat > /repos/control/conf/passwd <<'EOF'
[users]
tester = local-test-only
EOF
cat > /repos/sasl/conf/svnserve.conf <<'EOF'
[general]
anon-access = none
auth-access = write
realm = ankh-sasl-test
[sasl]
use-sasl = true
min-encryption = 0
max-encryption = 0
EOF
cat > /etc/sasl2/svn.conf <<'EOF'
pwcheck_method: auxprop
auxprop_plugin: sasldb
sasldb_path: /etc/svn-sasldb
mech_list: DIGEST-MD5
EOF
printf '%s' 'local-test-only' | saslpasswd2 -p -c -f /etc/svn-sasldb -u ankh-sasl-test tester
exec svnserve --daemon --foreground --root /repos
