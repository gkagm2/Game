var express = require('express'); // express ��� �ε�
var bodyParser = require("body-parser"); // �ٵ��ļ� ���ε�
var mysqlConfig = require("./mysqlConfig.js"); // mysql �������� �ε�
var app = express(); // express �� ����
var mysql = require("mysql"); // mysql��� �ε�
var conn = mysql.createConnection(mysqlConfig); // mysql ���ӻ���

app.use(bodyParser.json()); // �ٵ��ļ��� json() ���
app.use(bodyParser.urlencoded({ extended: true })); // �ٵ��ļ��� url���ڵ� ���.
console.log("����~~");
// �ּ�/new�� ������ ����� ó��...

// Insert Query
app.post('/new', function(req, res, err){
	if(req !== null){ // request(��û)�� null�� �ƴ϶��...
		console.log("1����~~");
		if(parseInt(req.body.Name) !== null){ // ��û�� �������� Name�� null�� �ƴ϶��...
			console.log("2����~~");
			var newQuery = "INSERT INTO UserInfoTable( Name, GId, Gold, Score, Level) VALUES(?, ?, ?, ?, ?)";
			var param = [ Name = req.body.Name, GId = req.body.GId, Gold = req.body.Gold, Score = req.body.Score, Level = req.body.Level ];
			console.log("3����~~");
			conn.query(newQuery, param, function(err, row, fields){
				if(!err){
					
					console.log("Data INSERT Success!!"); // insert ���� �� �޼���
					var resultQuery = "SELECT LAST_INSERT_ID();";
					conn.query(resultQuery, function(err, row, fields){
						if(!err){
							res.send(row[0]);
						} else {
							console.log(err);
						}
					});
				} else {
					console.log("error : " + err);
					res.send(err);
				}
			});
		} else {
			res.send(err);
		}
	} else {
		res.send(err);
	}
});

app.post('/checkId', function(req, res, err) {
	if(req !== null){
		var checkIdQuery = "SELECT UserInfoTable.Key FROM UserInfoTable WHERE UserInfoTable.Name = ?";
		var param = [Name = req.body.Name];
		conn.query(checkIdQuery, param, function(err, row, fields){
			if(!err){
				if(row.length === 0){
					res.send("NotExist");
				} else {
					res.send(row);
				}
			} else {
				res.send(err);
			}
		});
	} else {
		res.send(err);
	}
});


// Select Query
app.post('/select', function(req, res, err){
	console.log("[[1]]");
	if(req !== null) { // request(��û)�� null�� �ƴ϶��...
		console.log("[[2]]");
		var selectQuery = "SELECT * FROM UserInfoTable WHERE UserInfoTable.Key = ?";
		var param = [Key = req.body.Key];
		console.log("[[3]]");
		conn.query(selectQuery, param, function(err, row, fields){
			console.log("[[4]]");
			if(!err){
				console.log("[[5]]");
				if(row.length === 0){
					console.log("Key is Null");
					res.send("Key is Null")
				} else {
					console.log("select success[[6]]");
					res.send(row);
				}
			} else {
				res.send(err);
			}
		});
	} else {
		res.send(err);
	}
});

// Update Query
app.post('/update', function(req, res, err){
	if(req !== null){
		var updateQuery = "UPDATE UserInfoTable SET Gold = ?, Score = ? WHERE UserInfoTable.Key = ?";
		var param = [ Gold = req.body.Gold, Score = req.body.Score, Key = req.body.Key];
		conn.query(updateQuery, param, function(err, row, fields){
			if(!err){
				res.send("Update Complete");
			} else {
				res.send(err);
			}
		});
	} else {
		res.send(err);
	}
});

// Delete Query
app.post('/delete', function(req, res, err){
	console.log("delete 1");
	if(req !== null){
		console.log("delete 2");
		var deleteQuery = "DELETE FROM UserInfoTable WHERE UserInfoTable.Key = ?";
		var param = [Key = req.body.Key];
		console.log("delete 3");
		conn.query(deleteQuery, param, function(err, row, fields){
			console.log("delete 4");
			if(!err){
				console.log("delete 5");
				if(row.length === 0){
					console.log("Key is Null");
					res.send("Key is Null");
				} else {
					console.log("delete success");
					res.send(row);
				}
			} else {
				res.send(err);
			}
		});
	} else {
		res.send(err);
	}
});

app.listen(3001);
console.log('CRUD listening on port 3001');