db.createUser({
    user: "albumify-user",
    pwd: "albumify-password",
    roles: [{role: "readWrite", db: "albumify"}]
});