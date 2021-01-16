df_test <- read.csv(DANE TESTOWE)
df_train <- read.csv(DANE TRENINGOWE)
train_nb <- naiveBayes(label ~ . , data = df_train, laplace = 1)
pre_test <- predict(train_nb, df_test)
result_b <- cbind(df_test,pre_test)

val <- case_when(
result_b$label == result_b$pre_test & result_b$label == "normal"  ~ "TP",
result_b$label == result_b$pre_test & result_b$label == "attack"  ~ "TN",
result_b$label != result_b$pre_test & result_b$label == "attack"  ~ "FP",
result_b$label != result_b$pre_test & result_b$label == "normal"  ~ "FN")
result_b <- cbind(result_b,val)

table(result_b$val)

